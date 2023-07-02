
public class Zapis {
    // закрытые члены класса: фамилия, имя, телефон, дата рождения
    String Name_Famil;
    String Telefon;
    int[] Date = new int[3];
    int KolZap;

    // конструстор класса
    public Zapis() {

    }

    int GetColZap() {
        return KolZap;
    }

    public void SetPhoneNumber(String phoneNumber) {
        this.Telefon = phoneNumber;
    }

    String GetPhoneNumber() {
        return Telefon;
    }

    public void print() {
        System.out.print("\n" + Name_Famil + " родился " + Date[0] + "." + Date[1] + "." + Date[2]
                + ", его номер телефона: " + Telefon);
    }
}
