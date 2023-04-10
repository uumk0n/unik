import java.util.Scanner;
public class zad2 {
    //18) вычислить объем цилиндра, зная радиус основания и высоту; 
    public static void main(String[] args) throws Exception
    {
        //V = π·r²·h.
       final double pi = 3.14;

        Scanner in = new Scanner(System.in);

        System.out.print("Input radius: ");
        double r = in.nextDouble();
        
        System.out.print("Input H:");
        double h = in.nextDouble();

        double v = pi*Math.pow(r, 2)*Math.abs(h);

        System.out.print(v);
        in.close();
    }
}
