# Genshin.Planner.Demo

## TODO
- [x] 通过所需的材料和背包里已有的材料，计算出最优的刷取特定秘境的次数和合成材料的次数。
- [ ] 通过刷取特定秘境的次数，计算出哪一天应该刷取哪一个秘境才最有效率。

## 资源获取
`Data\Recipes.json`是从 genshin.honeyhunterworld.com 中抓取的，在控制台输入以下代码即可：
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
