class Zapis  // конструстор класса
    () {
    // закрытые члены класса: фамилия, имя, телефон, дата рождения
    var Name_Famil: String? = null
    var Telefon: String? = null
    var Date = IntArray(3)
    var KolZap = 0
    fun GetColZap(): Int {
        return KolZap
    }

    fun SetPhoneNumber(phoneNumber: String?) {
        Telefon = phoneNumber
    }

    fun GetPhoneNumber(): String? {
        return Telefon
    }

    fun print() {
        print(
            "\n" + Name_Famil + " родился " + Date[0] + "."  + Date[1] + "." + Date[2]
                    + ", его номер телефона: " + Telefon
        )
    }
}
