from typing import Optional, Union
from xml.etree.ElementTree import Element

HEAD = "<?xml version=\"1.0\" encoding=\"utf-8\"?>"

class Item:
    name: str
    props: dict[str, str]
    content: Optional[Union[str, 'Item', list['Item']]]

    def __init__(self, name: str, props: dict[str, str], content: Optional[Union[str, 'Item', list['Item']]]):
        self.name = name
        self.props = props
        self.content = content
    
    def parse(el: Element) -> 'Item':
        name = el.tag
        props = dict(el.attrib.items())
        content = None

        if len(el) == 0:
            content = el.text
        elif len(el) == 1:
            content = Item.parse(el[0])
        else:
            content = [Item.parse(it) for it in el]
        
        return Item(name, props, content)

    def to_dict(self):
        name = self.name
        props = self.props
        content = {}

        if self.content == None:
            content = None
        elif isinstance(self.content, str):
            content = self.content
        elif isinstance(self.content, Item):
            content = self.content.to_dict()
        else:
            content = [x.to_dict() for x in self.content]
        
        return {
            "name": name,
            "props": props,
            "content": content,
        }

    def to_xml(self, root_indent: int = 0, indent: int = 4):
        bind = " " * root_indent
        res = f"{bind}<{self.name}"

        if len(self.props) > 0:
            res += " "
            data = []

            for k, v in self.props.items():
                data.append(f"{k}=\"{v}\"")
            
            res += " ".join(data)
        
        if self.content == None:
            res += " />"
            return res
        
        res += ">"
        
        if isinstance(self.content, str):
            res += self.content
        elif isinstance(self.content, Item):
            ind = " " * (root_indent + indent)
            rind = " " * root_indent
            it = self.content.to_xml(root_indent + indent, indent)

            res += f"\n{it}\n{rind}"
        else:
            rind = " " * root_indent

            for item in self.content:
                it = item.to_xml(root_indent + indent, indent)
                res += f"\n{it}"
            
            res += f"\n{rind}"

        res += f"</{self.name}>"

        return res

class CSProj:
    attrs: dict[str, str]
    props: dict[str, Item]
    items: dict[str, list[Item]]

    def __init__(self, attrs: dict[str, str], props: dict[str, Item], items: dict[str, list[Item]]):
        self.attrs = attrs
        self.props = props
        self.items = items
    
    def parse(el: Element) -> 'CSProj':
        attrs = el.attrib
        props: dict[str, Item] = {}
        items: dict[str, list[Item]] = {}

        for item in el:
            if item.tag.lower() == "propertygroup":
                for child in item:
                    tmp: Item = Item.parse(child)

                    if tmp.name in props:
                        raise RuntimeError(f"Item {tmp.name} already exists in CSProj properties!")
                    else:
                        props[tmp.name] = tmp
            elif item.tag.lower() == "itemgroup":
                for child in item:
                    tmp: Item = Item.parse(child)

                    if tmp.name in items:
                        items[tmp.name].append(tmp)
                    else:
                        items[tmp.name] = [tmp]
        
        return CSProj(attrs, props, items)

    def to_dict(self):
        attrs = self.attrs
        props = {}
        items = {}

        for k, v in self.props.items():
            props[k] = v.to_dict()
        
        for k, v in self.items.items():
            items[k] = [x.to_dict() for x in v]

        return {
            "attrs": attrs,
            "props": props,
            "items": items,
        }

    def to_xml(self, indent: int = 4):
        res = HEAD + f"\n<Project"
        ind = " " * indent

        if len(self.attrs) > 0:
            res += " "
            data = []

            for k, v in self.attrs.items():
                data.append(f"{k}=\"{v}\"")
            
            res += " ".join(data)
        
        res += ">"

        # Properties
        res += f"\n{ind}<PropertyGroup>\n"
        
        for item in self.props.values():
            res += item.to_xml(indent * 2, indent) + "\n"
        
        res = res.strip()
        res += f"\n{ind}</PropertyGroup>"

        # Items

        res += f"\n\n{ind}<ItemGroup>\n"

        for items in self.items.values():
            for item in items:
                res += item.to_xml(indent * 2, indent) + "\n\n"

        res = res.strip()
        res += f"\n{ind}</ItemGroup>"
        res += "\n</Project>"
        
        return res