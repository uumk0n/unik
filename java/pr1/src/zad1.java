import java.util.Scanner;
public class zad1 {
    //8) вычислить высоту треугольника, зная две стороны треугольника и угол между ними; 
    public static void main(String[] args) throws Exception {
        Scanner in = new Scanner(System.in);
        System.out.print("Input first side: ");
        double a = in.nextDouble();

        System.out.print("Input second side: ");
        double b= in.nextDouble();

        System.out.print("Input corner: ");
        double coner= in.nextDouble();

        double s = (Math.abs(a)*Math.abs(b)*Math.sin(coner))/2;

        if(s==0)
        {
            in.close();
            return;
        }
        double h = ((a*b)*Math.sin(coner))/(2*s);

        System.out.print(h);
        in.close();
    }
}
