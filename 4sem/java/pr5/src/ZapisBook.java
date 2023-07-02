import java.util.ArrayList;
import java.util.Scanner;

public class ZapisBook {
    // закрытый массив записей
    ArrayList<Zapis> _mass;

    // конструктор класса, создает массив записей
    public ZapisBook(int KolZap) {
        _mass = new ArrayList<Zapis>(KolZap);
    }

    public void SortBynumber() {

        if (_mass.size() == 1)
            return;
        boolean sorted = false;
        Zapis temp;
        while (!sorted) {
            sorted = true;
            for (int i = 0; i < _mass.size() - 1; i++) {
                if (Long.parseLong(_mass.get(i).Telefon) > Long.parseLong(_mass.get(i + 1).Telefon)) {
                    temp = _mass.get(i);
                    _mass.remove(i);
                    _mass.add(i, _mass.get(i + 1));
                    _mass.remove(i + 1);
                    _mass.add(i + 1, temp);
                    sorted = false;
                }
            }
        }
        for (int i = 0; i < _mass.size() - 1; i++) {
            _mass.get(i).print();
        }

    }

    public void FindwSameDate() {
        if (_mass.size() == 1)
            return;
        System.out.print("input date: ");
        Scanner in = new Scanner(System.in);
        int[] date = new int[3];
        System.out.print("\n день: ");
        date[0] = in.nextInt();
        System.out.print("\n месяц: ");
        date[1] = in.nextInt();
        System.out.print("\n год: ");
        date[2] = in.nextInt();

        boolean same = false;
        for (int i = 0; i < _mass.size(); i++) {
            for (int j = 0; j < _mass.get(i).Date.length; j++) {
                if (_mass.get(i).Date[j] == date[j]) {
                    same = true;
                } else {
                    same = false;
                }
            }
            if (same)
                _mass.get(i).print();
        }

        in.close();
    }
}
