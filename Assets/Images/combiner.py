from PIL import Image
from xml.etree import ElementTree as ET

def run():
    im = Image.open('Flame.png')
    
    root = ET.parse('Flame.xml')
    sprites = root.findall('./sprite')

    for i in sprites:
        l = (int(i.attrib['x']),int(i.attrib['y']),int(i.attrib['x'])+int(i.attrib['w']),int(i.attrib['y'])+int(i.attrib['h']))
        new = im.crop(l)
        new.save('ShipSprites/'+i.attrib['n'])


if __name__ == '__main__':
    run()
