import java.util.Scanner;

class Shipment {
    int volume;
    int weight;

    public Shipment(int volume, int weight) {
        this.volume = volume;
        this.weight = weight;
    }

    public static Shipment fromInput(Scanner scanner) {
        int volume = scanner.nextInt();
        int weight = scanner.nextInt();
        return new Shipment(volume, weight);
    }

    public int calculateVolumetricCost() {
        return volume / 4;
    }

    public int calculateWeightCost() {
        return weight / 6;
    }

    public int more(Shipment other) {
        int finalCostA = Math.max(calculateVolumetricCost(), calculateWeightCost());
        int finalCostB = Math.max(other.calculateVolumetricCost(), other.calculateWeightCost());

        if (finalCostA > finalCostB) {
            return 1;
        } else if (finalCostA < finalCostB) {
            return -1;
        } else {
            return 0;
        }
    }
}

public class Main {
    public static void main(String[] args) {
        Scanner in = new Scanner(System.in);
        Shipment a = Shipment.fromInput(in);
        Shipment b = Shipment.fromInput(in);
        System.out.println(a.more(b));
    }
}
