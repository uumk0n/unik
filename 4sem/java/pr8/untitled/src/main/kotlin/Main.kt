import kotlinx.serialization.Serializable
import kotlinx.serialization.serializer
import kotlinx.serialization.decodeFromString
import kotlinx.serialization.json.Json
import java.io.File
import java.nio.file.Files
import java.nio.file.Paths
import java.io.FileReader
import java.util.*
import java.util.concurrent.ThreadLocalRandom

object App {
    @Throws(Exception::class)
    @JvmStatic
    fun main(args: Array<String>) {
        print("1.Ввести вручную\n2.Считать из файла\n3.Заполнить случайными элементами\n")
        val `in` = Scanner(System.`in`)
        val c = `in`.nextInt()
        when (c) {
            1 -> {
                val In = Scanner(System.`in`)
                print("Введите количество записей: ")
                val KolZap = In.nextInt()
                val zpB = ZapisBook(KolZap)
                var zap: Zapis
                run {
                    var i = 0
                    while (i < KolZap) {
                        zap = Zapis()
                        System.out.printf("\nВведите информацию по записи № %d", i + 1)
                        print("\nИмя фамилия: ")
                        zap.Name_Famil = In.next()
                        print("\nТелефон(без +): ")
                        zap.Telefon = In.next()
                        print("\nДата рождения: ")
                        print("\n день: ")
                        zap.Date[0] = In.nextInt()
                        print("\n месяц: ")
                        zap.Date[1] = In.nextInt()
                        print("\n год: ")
                        zap.Date[2] = In.nextInt()
                        zpB._mass.add(zap)
                        zap.KolZap = KolZap
                        System.out.printf("\nКоличество записей в книге: %d", zap.GetColZap())
                        i++
                    }
                }
                print("\nСписок всех записей: \n")
                var i = 0
                while (i < KolZap) {
                    zpB._mass[i].print()
                    i++
                }
                print("\nСортированный список по номеру: \n")
                zpB.SortBynumber()
                In.close()
            }

            2 -> {

                val json = File("C:/Users/uumk0/OneDrive/Desktop/PR8/untitled/file.json").readText()
                val records = Json.decodeFromString<List<Zapis>>(json)
                val zapBook = ZapisBook(records.toMutableList().size)

                for(i in records)
                {
                    zapBook._mass.add(i)
                }

                println("Список всех записей:")
                for (i in zapBook._mass.indices) {
                    zapBook._mass[i].print()
                }

                zapBook.FindwSameDate()

                val list = zapBook._mass.toList()
                writeListToJsonFile(list,"C:\\Users\\uumk0\\OneDrive\\Desktop\\PR8\\untitled\\output.json")
            }

            3 -> {
                val lexicon = "ABCDEFGHIJKLMNOPQRSTUVWXYZ"
                val numbers = "1234567890"
                val rand = Random()
                val identifiers: Set<String> = HashSet()
                val In = Scanner(System.`in`)
                print("Введите количество записей: ")
                val KolZap = In.nextInt()
                val zpB = ZapisBook(KolZap)
                var zap: Zapis
                run {
                    var i = 0
                    while (i < KolZap) {
                        zap = Zapis()
                        zap.Name_Famil = randomIdentifier(rand, lexicon, identifiers)
                        zap.Telefon = randomIdentifier(rand, numbers, identifiers)
                        zap.Date[0] = rand.nextInt(31)
                        zap.Date[1] = rand.nextInt(12)
                        zap.Date[2] = ThreadLocalRandom.current().nextInt(1923, 2023 + 1)
                        zpB._mass.add(zap)
                        zap.KolZap = KolZap
                        i++
                    }
                }
                System.out.printf("\nКоличество записей в книге: %d", zpB._mass[0].GetColZap())
                print("\nСписок всех записей: \n")
                var i = 0
                while (i < KolZap) {
                    zpB._mass[i].print()
                    i++
                }
                In.close()
            }
        }
        `in`.close()
    }

    fun writeListToJsonFile(list: List<Zapis>, fileName: String) {
        val json = Json.encodeToString(serializer<List<Zapis>>(), list)
        val file = File(fileName)
        file.writeText(json)
    }

    fun randomIdentifier(rand: Random, lexicon: String, identifiers: Set<String>): String {
        var builder = StringBuilder()
        while (builder.toString().length == 0) {
            val length = rand.nextInt(5) + 5
            for (i in 0 until length) {
                builder.append(lexicon[rand.nextInt(lexicon.length)])
            }
            if (identifiers.contains(builder.toString())) {
                builder = StringBuilder()
            }
        }
        return builder.toString()
    }
}