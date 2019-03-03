import openpyxl
import mysql.connector
import turkishnlp
from turkishnlp import detector

def karakter_temizle(dizi):
    for eleman in dizi:
        if not eleman.isalnum():
            dizi.remove(eleman)
    return dizi

def kelime_düzelt(dizi):
    nesne = detector.TurkishNLP()
    varmı=True
    try:
        open("words.pkl","r")
    except FileNotFoundError:
        varmı=False
    if not varmı:
        nesne.download()
    nesne.create_word_set()
    for i in range(len(dizi) - 1):
        kelime_kontrol = nesne.list_words(dizi[i])
        düzgün_kelime = nesne.auto_correct(kelime_kontrol)
        düzgün_kelime_son = " ".join(düzgün_kelime)
        dizi[i] = düzgün_kelime_son
    return dizi

def excel_al():
    kitap = openpyxl.load_workbook('kelime.xlsx')
    olumlu_sayfa=kitap.get_sheet_by_name('Olumlular')
    olumsuz_sayfa=kitap.get_sheet_by_name('Olumsuzlar')

    olumlu=[]
    olumsuz=[]

    for i in range(2,530):
        i=str(i)
        cell=olumlu_sayfa['A'+i].value
        olumlu.append(cell)

    for i in range(2,741):
        i=str(i)
        cell=olumsuz_sayfa['A'+i].value
        olumsuz.append(cell)
    kitap.close()

    return olumlu,olumsuz


def veritabani():
    mydb = mysql.connector.connect(
      host="localhost",
      user="root",
      passwd="123456",
      database="proje"
    )
    
    mycursor = mydb.cursor()
    mycursor.execute("SELECT * FROM twitter where id BETWEEN 1000 and 2500 limit 50")
    myresult = mycursor.fetchall()

    
    return myresult
 
def main():
    
    olumlu,olumsuz=excel_al()
    myresult=veritabani()
    """for x in myresult:
      yeni="]"+x[3]+"]"
      print(x)"""

    puandizi=[]
    for i in range(len(myresult)):
        puan=0
        yeni=myresult[i][3]
        print(yeni)
        yeni=yeni.split(" ")
        yeni = karakter_temizle(yeni)
        #yeni = kelime_düzelt(yeni)
        for j in range(len(yeni)):

            for k in range(len(olumlu)):
                if olumlu[k] in yeni[j]:
                    puan+=1
            for kk in range(len(olumsuz)):
                if olumsuz[kk] in yeni[j]:
                    puan-=1
                
        puandizi.append(puan)   
        print("skor :",puan)
        print("---------------------------------------------------------------------------------------------------------------------")
    
    print(puandizi)    
main()


