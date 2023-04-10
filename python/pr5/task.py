import random
import functions


import matplotlib.pyplot as plt
import numpy as np


random.seed(42)


print("random \n")
genMask = None
qrs = np.array(functions.generate_qr_code(genMask))
not_qrs = np.array(functions.genNoQrCode())
randomMaskW = functions.train(qrs, not_qrs)

print("not random \n")
genMask = 5
qrs = np.array(functions.generate_qr_code(genMask))
not_qrs = np.array(functions.genNoQrCode())
maskW = functions.train(qrs, not_qrs)

plt.figure()
plt.subplot(2, 2, 1)
plt.imshow(randomMaskW)
plt.subplot(2, 2, 2)
plt.imshow(maskW)
plt.show()