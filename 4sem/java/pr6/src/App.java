import java.util.Comparator;
import java.util.HashSet;
import java.util.List;
import java.util.Random;
import java.util.Scanner;
import java.util.Set;
import java.util.concurrent.ThreadLocalRandom;
import java.util.stream.Collectors;

public class App {
    public static void main(String[] args) throws Exception {
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

        System.out.println("\n \n Sortered by number: \n");
        List<Zapis> filtered = zpB._mass.stream()
                .filter(str -> str.GetPhoneNumber().length() > 5)
                .collect(Collectors.toList());

        for (int i = 0; i < filtered.size(); i++) {
            filtered.get(i).print();
        }

        System.out.println("\n \n Transform a collection using the map operator: \n ");
        List<Zapis> strNumbers = zpB._mass.stream()
                .map(num -> num)
                .collect(Collectors.toList());

        for (int i = 0; i < strNumbers.size(); i++) {
            strNumbers.get(i).print();
        }

        // System.out.println("\n \n Perform object processing using the peek operator: \n ");
        // List<Zapis> updated = zpB._mass.stream()
        //         .peek(person -> person.SetPhoneNumber(person.GetPhoneNumber() + "228123"))
        //         .collect(Collectors.toList());

        // for (int i = 0; i < updated.size(); i++) {
        //     updated.get(i).print();
        // }

        System.out.println("\n \n First 3: \n");
        List<Zapis> firstThree = zpB._mass.stream()
                .limit(3)
                .collect(Collectors.toList());
        for (int i = 0; i < firstThree.size(); i++) {
            firstThree.get(i).print();
        }
        System.out.println("\n \n Last 4:\n");
        List<Zapis> last4 = zpB._mass.stream()
                .skip(3)
                .collect(Collectors.toList());
        for (int i = 0; i < last4.size(); i++) {
            last4.get(i).print();
        }

        System.out.print("\n \n Convert a collection to an array: \n");

        Zapis[] arrayZap = zpB._mass.toArray(new Zapis[zpB._mass.size()]);

        for (int i = 0; i < arrayZap.length; i++) {
            arrayZap[i].print();
        }

        System.out.println("\n \n Oldest: \n");
        Zapis oldest = zpB._mass.stream()
                .min(Comparator.comparingInt(Zapis::GetDate))
                .orElse(null);

        oldest.print();

        System.out.println("\n \n Check that one phone number in the collection is even: \n");
        boolean hasEven = zpB._mass.stream()
                .anyMatch(num -> Long.parseLong(num.GetPhoneNumber()) % 2 == 0);

        System.out.print(hasEven);

        System.out.println("\n \n Check that all phone numbers in the collection are even: \n");
        boolean allGreaterThanZero = zpB._mass.stream()
                .allMatch(num -> Long.parseLong(num.GetPhoneNumber()) % 2 == 0);

        System.out.print(allGreaterThanZero);
        In.close();
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
