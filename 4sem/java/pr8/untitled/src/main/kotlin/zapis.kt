import kotlinx.serialization.Serializable
import java.util.*

@Serializable
data class ZapisData(
    val Name_Famil: String,
    val Telefon: String,
    val Date: List<Int>
)

@Serializable
data class ZapisBookData(val zapisList: List<ZapisData>)

@Serializable
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

class ZapisBook(KolZap: Int) {
    // закрытый массив записей
    var _mass: ArrayList<Zapis>

    // конструктор класса, создает массив записей
    init {
        _mass = ArrayList(KolZap)
    }

    fun SortBynumber() {
        if (_mass.size == 1) return
        var sorted = false
        var temp: Zapis
        while (!sorted) {
            sorted = true
            for (i in 0 until _mass.size - 1) {
                if (_mass[i].Telefon!!.toLong() > _mass[i + 1].Telefon!!.toLong()) {
                    temp = _mass[i]
                    _mass.removeAt(i)
                    _mass.add(i, _mass[i + 1])
                    _mass.removeAt(i + 1)
                    _mass.add(i + 1, temp)
                    sorted = false
                }
            }
        }
        for (i in 0 until _mass.size - 1) {
            _mass[i].print()
        }
    }

    fun FindwSameDate() {
        if (_mass.size == 1) return
        print("input date: ")
        val `in` = Scanner(System.`in`)
        val date = IntArray(3)
        print("\n день: ")
        date[0] = `in`.nextInt()
        print("\n месяц: ")
        date[1] = `in`.nextInt()
        print("\n год: ")
        date[2] = `in`.nextInt()
        var same = false
        for (i in _mass.indices) {
            for (j in _mass[i].Date.indices) {
                same = if (_mass[i].Date[j] == date[j]) {
                    true
                } else {
                    false
                }
            }
            if (same) _mass[i].print()
        }
        `in`.close()
    }
}

