import java.util.Scanner;
public class zad3 {
    //28)вычислить y = x/cos (x) + x2/sin (x);
    public static void main(String[] args) throws Exception {
        Scanner in = new Scanner(System.in);

        System.out.print("Input x: ");
        double x = in.nextDouble();

        if(x==0)
        {
            in.close();
            return;
        }
        double y = (x/(Math.cos(x))) + ((x*2)/(Math.sin(x)));

        System.out.print(y);
        in.close();
    }

}
