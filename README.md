# Genshin.Planner.Demo

## TODO
[x] ͨ������Ĳ��Ϻͱ��������еĲ��ϣ���������ŵ�ˢȡ�ض��ؾ��Ĵ����ͺϳɲ��ϵĴ�����
[ ] ͨ��ˢȡ�ض��ؾ��Ĵ������������һ��Ӧ��ˢȡ��һ���ؾ�������Ч�ʡ�

## ��Դ��ȡ
`Data\Recipes.json`�Ǵ� genshin.honeyhunterworld.com ��ץȡ�ģ��ڿ���̨�������´��뼴�ɣ�
```javascript
sortable_data[0].map(item => {
    let name = new DOMParser().parseFromString(item[1], "text/xml").documentElement.innerHTML
    let formulas = Array.from(new DOMParser().parseFromString(item[4], "text/html").documentElement.querySelectorAll("body > div"))
        .map(rawFormula =>
            Array.from(rawFormula.querySelectorAll("a > div"))
                .map(rawItem => {
                    return {
                        name: rawItem.querySelector("img").getAttribute("alt"),
                        quantity: Number.parseInt(rawItem.querySelector("span").innerHTML)
                    }
                })
        )
    return formulas.map(formula => {
        return {
            name: name,
            formula: formula,
        }
    })
}).flat()
```