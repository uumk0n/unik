import openpyxl
import csv

# создаём файл ексель с таблицей
n = len("Ilya")
m = len("Konyaev")
wb = openpyxl.Workbook()
ws = wb.active
for i in range(1, n+1):
    row = []
    for j in range(1, m+1):
        cell_value = (i-1)*m + j
        row.append(cell_value)
    ws.append(row)
wb.save("table.xlsx")

# читаем файл ексель и переделываем в csv
rows = []
with open('table.csv', 'w', newline='') as csvfile:
    csvwriter = csv.writer(csvfile)
    for row in ws.iter_rows(values_only=True):
        csvwriter.writerow(row)
        rows.append(row)

# читаем csv файл
with open('table.csv', newline='') as csvfile:
    csvreader = csv.reader(csvfile)
    rows = [list(map(int, row)) for row in csvreader]

# считаем d и C
A = rows
B = list(map(list, zip(*A)))
C = [[sum([a*b for a, b in zip(row, col)]) for col in B] for row in A]
sum_c = sum(sum(C, []))
d = [sum(col) for col in B]
sum_d_squared = sum([i**2 for i in d])

# выводим
print("C = AB: ", sum_c)
print("d: ", sum_d_squared)

# C = AB:  22092
# d:  23996
