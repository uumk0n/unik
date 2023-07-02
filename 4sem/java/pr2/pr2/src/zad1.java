import java.util.Scanner;
public class zad1 {
    //задание 7
    public static void main(String[] args) throws Exception {
        Scanner in = new Scanner(System.in);
        System.out.print("Input numbers a,b,c,x usage Space: ");

        String input = in.nextLine();
        String[] parts = input.split(" ");
        double F=0;
        for (int i=0;i<parts.length;++i) {
            try {
                if(parts.length>4)
                {
                    System.out.print("a lot of numbers");
                    break;
                }
            double a = Double.parseDouble( parts[i].replace(",",".") );
            double b = Double.parseDouble( parts[i+1].replace(",",".") );
            double c = Double.parseDouble( parts[i+2].replace(",",".") );
            double x = Double.parseDouble( parts[i+3].replace(",",".") );
            
            if(a<0&&x!=0)
            {
                F=(a*Math.pow(x, 2)+(Math.pow(b, 2)*x));
            }
            else if(a>0&&x==0)
            {
                F=x-(a/(x-c));
            }
            else 
            {
                F=1+(x/c);
            }
            in.close();
            System.out.print(F);
            break;

            }catch (Exception e) {
                System.out.print("not enough numbers");
                in.close();
            }
 
        }

    }
}
