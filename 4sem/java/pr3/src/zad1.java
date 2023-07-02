import java.util.Scanner;

public class zad1 {
    //найдите сумму целых чисел из промежутка от A до B кратных 4
    public static void main(String[] args) throws Exception {
        Scanner in = new Scanner(System.in);

        System.out.print("Input a: ");
        int a = in.nextInt();
        System.out.print("Input b: ");
        int b =in.nextInt();

        int sum =0;

        for(int i=a;i<b;i++)
            sum+=i;
        

        System.out.print(sum);
        in.close();
    }
}
