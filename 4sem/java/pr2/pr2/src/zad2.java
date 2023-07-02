import java.util.Scanner;

public class zad2 {
    //1) 7. Написать программу, которая требует ввода времени дня и, в 
    //зависимости от введенного значения, желает доброго утра, доброго дня, 
    //доброго вечера или спокойной ночи.
    public static void main(String[] args) throws Exception {
        Scanner in = new Scanner(System.in);

        String input = in.nextLine();
        String[] parts = input.split(":");

        String hours="";

        for(int i=0;i<1;i++)
            hours=parts[i];
        
        
        int h= Integer.parseInt(hours);
if (h > 3 && h < 7) System.out.print("Доброй ночи");
if (h > 6 && h < 12) System.out.print("Доброе утро");
if (h > 11 && h < 17) System.out.print("Добрый день");
if (h > 16 && h < 24) System.out.print("Добрый вечер");
if (h > 23 || h < 4 ) System.out.print("Доброй ночи");
        in.close();
    }
}
