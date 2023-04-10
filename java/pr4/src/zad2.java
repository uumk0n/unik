import java.util.Scanner;

public class zad2 {
    public static void main(String[] args) throws Exception {
        Scanner in = new Scanner(System.in);

        System.out.print("Input cols count ");
        int n = in.nextInt();
        System.out.print("Input rows count ");
        int m = in.nextInt();

        System.out.print("Input range ");
        double min = in.nextDouble();
        double max = in.nextDouble();
        double [][] arr = new double[n][m];

        for(int i=0;i<n;i++)
        {
            for(int j=0;j<m;j++)
            {
                arr[i][j]=Math.round(rnd(min, max));
                System.out.print(arr[i][j] + "    ");
            }
            System.out.println();
        }

        for (int i = 0; i < n; i++)
{
    for (int j = 0; j < m; j++)
    {
        for (int s = i; s < n; s++)
        {
            for (int t = j + 1; t < m; t++)
                if (arr[i][j] == arr[s][t])
                {
                    System.out.print("\n Same elements at pos:\n");
                    System.out.print(i+","+j+" and ");
                    System.out.print(s+","+t);
                }
            }
        }
    }

        
            in.close();
    }   

    public static double rnd(double min, double max){
        max -= min;
	return (Math.random() * ++max) + min;
    }
}
