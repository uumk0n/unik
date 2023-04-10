import java.util.Scanner;

public class zad3 {
    //22.Написать программу, которая по введенному числу от 1 
    //до 12 (номеру месяца) выдает все приходящиеся на этот месяц праздничные дни (например, если 
    //введено число 1, то должно получиться 1 января — Новый год, 7 января — Рождество).
    public static void main(String[] args) throws Exception {
        Scanner in = new Scanner(System.in);

        System.out.print("Input month:");

        int i =in.nextInt();

        switch (i) {
            case 1:
            {
                System.out.print("1, 2, 3, 4, 5 января – Новогодние каникулы\n");
                System.out.print("7 января – Рождество Христово:");
                    break;
            }
            case 2:
            {
                System.out.print("23 февраля – День защитника Отечества\n");
                break;
            }
            case 3:
            {
                System.out.print("8 марта – Международный женский день\n");
                break;
            }
            case 4:
            {
                System.out.print("1 апреля - День смеха\n");
                break;
            }
            case 5:
            {
                System.out.print("1 мая – Праздник Весны и Труда\n");
                System.out.print("9 мая – День Победы\n");
                break;
            }
            case 6:
            {
                System.out.print("12 июня - День России\n");
                break;
            }
            case 7:
            case 8:
            {
                System.out.print("Никаких праздников(((");
                break;
            }
            case 10:
            {
                System.out.print("Последнее воскресенье - День матери");
                break;
            }
            case 11:
            {
                System.out.print("4 ноября - День народного единства\n");
                break;
            }
            case 12:
            {
                System.out.print("31 Декабря - выходной\n");
                break;
            }
        
            default:
            in.close();
            break;
        }
    }
    
}
