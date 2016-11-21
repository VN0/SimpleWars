from PIL import Image

def run():
    im = Image.open('Atmospheres.png')
    x = 0

    for i in range(0,512,32):
        l = (i,0,i+32,512)
        new = im.crop(l)
        new.save('AtmosphereSmearth.png')
        break


if __name__ == '__main__':
    run()
