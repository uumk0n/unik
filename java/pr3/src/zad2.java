

public class zad2 {
    
    public static long factorial(int number) {
        long result = 1;

        for (int factor = 2; factor <= number; factor++) {
            result *= factor;
        }

        return result;
    }
    public static void main(String[] args) throws Exception {
         //final double e = 0.001;

         double sum =0;
         double a =1;
         int n =1;

         while (n<=10)
         {
            a=(Math.log(factorial(n)))%(Math.pow(n, 2));
            sum+=a;
            n++;
         }
         System.out.print(sum);
    }
}
