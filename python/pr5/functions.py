import string
from typing import Optional
import io
from tqdm import tqdm
import segno
import random

def perp(sensors_mas: list[list[int]], weights_inner: list[list[int]], bias: int) -> bool:
    sensors_sum = 0
    for i in range(len(sensors_mas)):
        for j in range(len(sensors_mas[i])):
            sensors_sum += sensors_mas[i][j] * weights_inner[i][j]
    return sensors_sum >= bias

def genRandomStr(length: int) -> str:
    '''Generate a random string'''
    return ''.join(random.choices(string.ascii_letters + string.digits, k=length))

def generate_qr_code(mask: Optional[int]) -> list[list[int]]:
    return_mas_qr_code = []
    for _ in tqdm(range(100), desc='Создаём qrcode'):
        qrcode = segno.make(genRandomStr(970), mask=mask)
        buffer = io.StringIO()
        qrcode.save(buffer, kind="txt", border=0)
        qr_code = [[int(bin_digit) for bin_digit in line] for line in buffer.getvalue().split("\n")][:-1]
        return_mas_qr_code.append(qr_code)
    return return_mas_qr_code

def genNoQrCode(n: int = 100, size: int = 177) -> list[list[int]]:
    return_mas_no_qr_code = []
    for _ in tqdm(range(n), desc='создаём 100 сломанный qrcode'):
        inner_mas_no_qr_code = [[random.randint(0, 1) for _ in range(size)] for _ in range(size)]
        return_mas_no_qr_code.append(inner_mas_no_qr_code)
    return return_mas_no_qr_code

def getWaterMar(
    qr_code: list[list[int]],
    start_point_width: int,
    final_point_width: int,
    start_point_down: int,
    final_point_down: int,
) -> list[list[int]]:
    '''&'''
    watermark = []
    for i in range(start_point_width, final_point_width):
        row = qr_code[i][start_point_down:final_point_down]
        watermark.append(row)
    return watermark

def checkQrCode(sensor: list[list[int]]) -> bool:
    weights = [
        [1, 1, 1, 1, 1, 1, 1],
        [1, 0, 0, 0, 0, 0, 1],
        [1, 0, 1, 1, 1, 0, 1],
        [1, 0, 1, 1, 1, 0, 1],
        [1, 0, 1, 1, 1, 0, 1],
        [1, 0, 0, 0, 0, 0, 1],
        [1, 1, 1, 1, 1, 1, 1],
    ]
    results = [getWaterMar(sensor, x, x + 7, y, y + 7) for x, y in [(0, 0), (170, 0), (0, 170)]]
    return all(r == weights for r in results)

def train(qr_codes, not_qr_codes):
    weights = [[0] * 177 for _ in range(177)]
    amount_of_epoches = 20
    steps = 200
    learning_step = 1
    for epoch in tqdm(range(amount_of_epoches), desc='считаем эпохи'):
        for step in range(steps):
            random_num = random.randint(0, 1)
            train_element = 0
            if random_num == 0:
                train_element = not_qr_codes[random.randint(0, 99)]
            else:
                train_element = qr_codes[random.randint(0, 99)]
            result = perp(train_element, weights, 5000)
            if result and random_num == 0:
                for i in range(len(train_element)):
                    for j in range(len(train_element[i])):
                        if train_element[i][j] == 1:
                            weights[i][j] -= learning_step
            elif not result and random_num == 1:
                for i in range(len(train_element)):
                    for j in range(len(train_element[i])):
                        if train_element[i][j] == 1:
                            weights[i][j] += learning_step
    return weights