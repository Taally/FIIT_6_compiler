import codecs

def gen_toc(path,order):
    toc = "" # table of content
    for i, name in enumerate(order):
        file = codecs.open(path+name+".md", "r", "utf_8_sig")
        title = file.readline()
        if (len(title) < 3):
            title = file.readline()
        title = title.replace('## ','')
        title = title.strip()
        file.close()
        toc += "[" + str(i+1) + ". " + title + "]" + "(#" + name[2:] + ")\n\n"
    return toc

def main():
    path="Documentation/"
    order = []
    with open(path+"order.txt", "r") as order_file:
        for line in order_file:
            order.append(line.strip())
    toc = gen_toc(path,order)

    res = codecs.open(path+"Documentation.md", "w", "utf_8_sig")
    res.write(toc)
    res.write("\n")
    for name in order:
        file = codecs.open(path+name+".md", "r", "utf_8_sig")
        header = file.readline()
        content = header + " {#" + name[2:] + "}\n\n"
        content += "".join(file.readlines()[1:])
        file.close()
        res.write(content)
    res.close()

if __name__ == "__main__":
    main()
