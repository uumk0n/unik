import java.io.BufferedReader;
import java.io.FileReader;
import java.util.ArrayList;
import java.util.HashSet;
import java.util.Random;
import java.util.Scanner;
import java.util.Set;
import java.util.concurrent.ThreadLocalRandom;

public class App {
    public static void main(String[] args) throws Exception {
        System.out.print("1.Ввести вручную\n2.Считать из файла\n3.Заполнить случайными элементами\n");
        Scanner in = new Scanner(System.in);
        int c = in.nextInt();
        switch (c) {
            case 1: {
                Scanner In = new Scanner(System.in);
                System.out.print("Введите количество записей: ");
                int KolZap = In.nextInt();
                ZapisBook zpB = new ZapisBook(KolZap);

                Zapis zap;
                for (int i = 0; i < KolZap; i++) {
                    zap = new Zapis();
                    System.out.printf("\nВведите информацию по записи № %d", i + 1);
                    System.out.print("\nИмя фамилия: ");
                    zap.Name_Famil = In.next();
                    System.out.print("\nТелефон(без +): ");
                    zap.Telefon = In.next();
                    System.out.print("\nДата рождения: ");
                    System.out.print("\n день: ");
                    zap.Date[0] = In.nextInt();
                    System.out.print("\n месяц: ");
                    zap.Date[1] = In.nextInt();
                    System.out.print("\n год: ");
                    zap.Date[2] = In.nextInt();
                    zpB._mass.add(zap);
                    zap.KolZap = KolZap;
                    System.out.printf("\nКоличество записей в книге: %d", zap.GetColZap());
                }

                System.out.print("\nСписок всех записей: \n");
                for (int i = 0; i < KolZap; i++) {
                    zpB._mass.get(i).print();
                }

                System.out.print("\nСортированный список по номеру: \n");
                zpB.SortBynumber();
                In.close();
                break;
            }
            case 2: {
                ArrayList<String> list = new ArrayList<String>();
                FileReader fr = new FileReader("/home/uumkon/Рабочий стол/university/4sem/java/pr5/bin/test.txt");
                Zapis zp;
                BufferedReader reader = new BufferedReader(fr);
                String line = reader.readLine();
                while (line != null) {

                    list.add(line);
                    line = reader.readLine();
                }
                ZapisBook zapBook = new ZapisBook(list.size());
                for (int i = 0; i < list.size(); i++) {
                    zp = new Zapis();
                    String[] listitem = list.get(i).split(";");
                    for (int j = 0; j < listitem.length - 2;) {
                        zp.Name_Famil = listitem[j];
                        zp.Telefon = listitem[j + 1];
                        String dates = listitem[j + 2];
                        String[] date = dates.split("-");
                        for (int k = 0; k < date.length - 1;) {
                            zp.Date[0] = Integer.parseInt(date[k]);
                            zp.Date[1] = Integer.parseInt(date[k + 1]);
                            zp.Date[2] = Integer.parseInt(date[k + 2]);
                            break;
                        }
                    }
                    zapBook._mass.add(zp);
                }
                System.out.print("\nСписок всех записей: \n");
                for (int i = 0; i < list.size(); i++) {
                    zapBook._mass.get(i).print();
                }
                zapBook.FindwSameDate();
                reader.close();
                break;
            }

            case 3: {
                final String lexicon = "ABCDEFGHIJKLMNOPQRSTUVWXYZ";
                final String numbers = "1234567890";
                final java.util.Random rand = new java.util.Random();

                final Set<String> identifiers = new HashSet<String>();

                Scanner In = new Scanner(System.in);
                System.out.print("Введите количество записей: ");
                int KolZap = In.nextInt();
                ZapisBook zpB = new ZapisBook(KolZap);

                Zapis zap;
                for (int i = 0; i < KolZap; i++) {
                    zap = new Zapis();

                    zap.Name_Famil = randomIdentifier(rand, lexicon, identifiers);

                    zap.Telefon = randomIdentifier(rand, numbers, identifiers);

                    zap.Date[0] = rand.nextInt(31);
                    zap.Date[1] = rand.nextInt(12);
                    zap.Date[2] = ThreadLocalRandom.current().nextInt(1923, 2023 + 1);

                    zpB._mass.add(zap);
                    zap.KolZap = KolZap;
                }
                System.out.printf("\nКоличество записей в книге: %d", zpB._mass.get(0).GetColZap());
                System.out.print("\nСписок всех записей: \n");
                for (int i = 0; i < KolZap; i++) {
                    zpB._mass.get(i).print();
                }
                In.close();
                break;
            }
        }

        in.close();
    }

    public static String randomIdentifier(Random rand, String lexicon, Set<String> identifiers) {
        StringBuilder builder = new StringBuilder();
        while (builder.toString().length() == 0) {
            int length = rand.nextInt(5) + 5;
            for (int i = 0; i < length; i++) {
                builder.append(lexicon.charAt(rand.nextInt(lexicon.length())));
            }
            if (identifiers.contains(builder.toString())) {
                builder = new StringBuilder();
            }
        }
        return builder.toString();
    }

}