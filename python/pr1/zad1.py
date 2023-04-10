x1=int(input())
x2=int(input())
y1=int(input())
y2=int(input())

if not((x1>=1 and x1<=8) or (y1>=1 and y1<=8) or (x2>=1 and x2<=8) or (y2>=1 and y2<=8)):
    print("wrong number")

if x1==y1 or x2==y2:
    print("Yes")
else:
    print("No")
