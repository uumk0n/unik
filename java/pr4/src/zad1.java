import java.util.Scanner;
public class zad1 {
    public static void main(String[] args) throws Exception {
        Scanner in = new Scanner(System.in);
        System.out.print("Input size mass ");
        int n=in.nextInt();
        System.out.print("Input range ");
        double min = in.nextDouble();
        double max = in.nextDouble();
        double [] t = new double[2*n];
        double  buf=0;
        for (int i = 0; i < t.length; i++) {
            t[i] = ((rnd(min, max)));
            System.out.println(t[i]);
        }
        sort(t, buf, n);
        print(t);
        in.close();
    }
    
    public static double rnd(double min, double max){
        max -= min;
	return (Math.random() * ++max) + min;
    
    }
    public static void sort(double[] arr, double buf, int size){
        for(int i=0;i<size;i++)
        {
            buf=arr[i+size];
            arr[i+size]=arr[i];
            arr[i]=buf;
        }
    }
    public static void print(double[] arr){
        System.out.println("\n sorter array: \n");
        for (int i = 0; i < arr.length; i++) {
            System.out.println(arr[i]);
        }   
    }
    
}
