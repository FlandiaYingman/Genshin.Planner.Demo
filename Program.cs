using System.Reflection;
using QuickType;
using QuickType.Materials;
using QuickType.Recipe;
using Genshin.Planner.Demo.LinearProgrammingSolver;
using static System.MidpointRounding;

namespace Genshin.Planner.Demo
{
    // Data from: https://genshindb-ia.netlify.app/
    internal record Material(string Name, string Id, int Rarity)
    {
        public static readonly List<Material> Materials = LoadMaterials();

        private static List<Material> LoadMaterials()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Genshin.Planner.Demo.Data.Materials.json");
            if (stream == null) throw new FileNotFoundException();
            return MaterialJson.FromJson(new StreamReader(stream).ReadToEnd())
                .Select(i => new Material(i.Name, i.Dupealias ?? i.Name, (int?)i.Rarity ?? 0))
                .ToList();
        }

        public static Material? GetMaterial(string name)
        {
            return Materials.Find(i => i.Name == name);
        }

        public Dictionary<Domain, Drop> GetSourcesDomain()
        {
            var w = Domain.WeaponDomains
                .Where(x => x.Drops.Any(y => y.Material == this))
                .ToDictionary(x => x, x => x.Drops.Find(y => y.Material == this)!);
            var t = Domain.TalentDomains
                .Where(x => x.Drops.Any(y => y.Material == this))
                .ToDictionary(x => x, x => x.Drops.Find(y => y.Material == this)!);
            return new Dictionary<Domain, Drop>()
                .Concat(w)
                .Concat(t)
                .ToDictionary(x => x.Key, x => x.Value);
        }

        public List<Recipe> GetSourceRecipe()
        {
            return Recipe.Recipes.FindAll(x => x.Output == this);
        }

        public Dictionary<Recipe, RecipeInput> GetUsagesRecipe()
        {
            return Recipe.Recipes
                .Where(x => x.Inputs.Any(y => y.Material == this))
                .ToDictionary(x => x, x => x.Inputs.Find(y => y.Material == this)!);
        }
    }

    internal record Drop(Material Material, double QuantityAvg);

    // Data from: https://genshindb-ia.netlify.app/
    internal record Domain(string Name, int Level, List<Drop> Drops)
    {
        // Data from: https://docs.google.com/spreadsheets/d/1RcuniapqS6nOP05OCH0ui10Vo3bWu0AvFbhgcHzTybY
        // 某稀有度的任意武器突破材料在某等级的对应秘境的平均掉落量。第一个维度为 0-3 的秘境等级，第二个维度为 0-3 的材料稀有度。
        private static readonly double[][] WeaponMaterialDropAvg =
        {
            new[] { 4.79, 0.00, 0.00, 0.00 },
            new[] { 2.71, 2.00, 0.00, 0.00 },
            new[] { 2.23, 2.78, 0.22, 0.00 },
            new[] { 2.20, 2.40, 0.64, 0.07 }
        };

        // 某稀有度的任意天赋培养材料在某等级的对应秘境的平均掉落量。第一个维度为 0-3 的秘境等级，第二个维度为 0-2 的材料稀有度。
        private static readonly double[][] TalentMaterialDropAvg =
        {
            new[] { 3.18, 0.00, 0.00 },
            new[] { 2.53, 1.00, 0.00 },
            new[] { 1.79, 2.00, 0.00 },
            new[] { 2.20, 1.97, 0.23 }
        };

        public static readonly List<Domain> WeaponDomains = LoadWeaponDomains();

        private static List<Domain> LoadWeaponDomains()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Genshin.Planner.Demo.Data.WeaponDomains.json");
            if (stream == null) throw new FileNotFoundException();
            return DomainJson.FromJson(new StreamReader(stream).ReadToEnd())
                .Select(i =>
                {
                    var dl = ParseDomainLevel(i.Name);
                    var drops = i.Rewardpreview.Select(j =>
                    {
                        var material = Material.GetMaterial(j.Name)!;
                        if (j.Count != null)
                        {
                            return new Drop(material, (double)j.Count);
                        }
                        else
                        {
                            var mr = material.Rarity - 2;
                            return new Drop(material, WeaponMaterialDropAvg[dl][mr]);
                        }
                    }).ToList();
                    return new Domain(i.Name, dl, drops);
                })
                .ToList();
        }

        public static readonly List<Domain> TalentDomains = LoadTalentDomains();

        private static List<Domain> LoadTalentDomains()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Genshin.Planner.Demo.Data.TalentDomains.json");
            if (stream == null) throw new FileNotFoundException();
            return DomainJson.FromJson(new StreamReader(stream).ReadToEnd())
                .Select(i =>
                {
                    var dl = ParseDomainLevel(i.Name);
                    var drops = i.Rewardpreview.Select(j =>
                    {
                        var material = Material.GetMaterial(j.Name)!;
                        if (j.Count != null)
                        {
                            return new Drop(material, (double)j.Count);
                        }
                        else
                        {
                            var mr = material.Rarity - 2;
                            return new Drop(material, TalentMaterialDropAvg[dl][mr]);
                        }
                    }).ToList();
                    return new Domain(i.Name, dl, drops);
                })
                .ToList();
        }

        private static int ParseDomainLevel(string domainName)
        {
            if (domainName.EndsWith(" I")) return 0;
            else if (domainName.EndsWith(" II")) return 1;
            else if (domainName.EndsWith(" III")) return 2;
            else if (domainName.EndsWith(" IV")) return 3;
            else return -1;
        }
    }

    internal record RecipeInput(Material Material, double Quantity);

    // Data from: https://genshin.honeyhunterworld.com/fam_crafted/?lang=CHS
    internal record Recipe(string Id, Material Output, List<RecipeInput> Inputs)
    {
        public static List<Recipe> Recipes = LoadRecipe();

        private static List<Recipe> LoadRecipe()
        {
            using var stream = Assembly.GetExecutingAssembly().GetManifestResourceStream("Genshin.Planner.Demo.Data.Recipes.json");
            if (stream == null)
            {
                throw new FileNotFoundException();
            }

            var recipeCount = new Dictionary<string, int>();
            return RecipeJson.FromJson(new StreamReader(stream).ReadToEnd())
                .Select(i =>
                {
                    if (recipeCount.ContainsKey(i.Name)) recipeCount[i.Name]++;
                    else recipeCount[i.Name] = 0;
                    var output = Material.GetMaterial(i.Name)!;
                    var inputs = i.Formula.Select(j => new RecipeInput(Material.GetMaterial(j.Name)!, j.Quantity)).ToList();
                    var recipeId = $"{i.Name}（合成 #{recipeCount[i.Name]}）";
                    return new Recipe(recipeId, output, inputs);
                })
                .ToList();
        }
    }

    internal class Program
    {
        // Program Input
        // 所需的材料
        private static readonly Tuple<string, int>[] Demand =
        {
            new("「繁荣」的哲学", 10),
            new("「抗争」的哲学", 20),
            new("「黄金」的哲学", 20),
        };

        // 已有的材料
        static readonly Tuple<string, int>[] Stock =
        {
            new("摩拉", 100000000),
            new("「黄金」的指引", 60),
        };

        private static void Main()
        {
            CalculateAction();
        }

        private static void CalculateAction()
        {
            var solver = new LpSolver();

            // 用一个恒定为1的整形变量来表示背包里物品的系数。
            var vStock = solver.CreateVariable("stock");
            vStock.LowerBound = 1.0;
            vStock.UpperBound = 1.0;
            vStock.ObjectiveCoefficient = 0;

            // 对于每种行动（秘境或合成），都赋一个整形变量代表。
            Domain.WeaponDomains.ForEach(x =>
            {
                var v = solver.CreateVariable(x.Name);
                v.LowerBound = 0;
                v.ObjectiveCoefficient = DomainCost(x.Level);
            });
            Domain.TalentDomains.ForEach(x =>
            {
                var v = solver.CreateVariable(x.Name);
                v.LowerBound = 0;
                v.ObjectiveCoefficient = DomainCost(x.Level);
            });
            Recipe.Recipes.ForEach(x =>
            {
                var v = solver.CreateVariable(x.Id);
                v.LowerBound = 0;
                v.ObjectiveCoefficient = 0.1; // 为合成添加极小的代价，以防止合成不必要的材料。
            });

            // 对于每种材料，添加最低为0的约束。特别地，对于需要的材料，添加最低为需求量的约束。
            // 每种材料的计算公式为：各行动的次数乘以其获得材料的期望值，再加上已有的材料
            Material.Materials.ForEach(x =>
            {
                var constraint = solver.CreateConstraint(x.Id);
                constraint.LowerBound = 0;
                constraint.UpperBound = double.PositiveInfinity;

                // 添加约束下界为所需材料数
                var demandMaterial = Array.Find(Demand, y => y.Item1 == x.Name);
                if (demandMaterial != null)
                    constraint.LowerBound = demandMaterial.Item2;

                // 添加背包里已有的材料数
                var stockMaterial = Array.Find(Stock, y => y.Item1 == x.Name);
                if (stockMaterial != null)
                    constraint.SetCoefficient("stock", stockMaterial.Item2);

                // 添加秘境掉落的材料期望
                x.GetSourcesDomain().ToList().ForEach(y =>
                {
                    var (domain, drop) = y;
                    constraint.SetCoefficient(domain.Name, drop.QuantityAvg);
                });
                // 添加合成消耗的材料
                x.GetUsagesRecipe().ToList().ForEach(y =>
                {
                    var (recipe, input) = y;
                    constraint.SetCoefficient(recipe.Id, -input.Quantity);
                });
                // 添加合成出的材料
                x.GetSourceRecipe().ForEach(y => { constraint.SetCoefficient(y.Id, 1.0); });
            });

            var status = solver.Solve(LpSolver.ProblemType.Minimize);

            Console.WriteLine(status);
            // 由于：一、此规划不是整数规划；二、cost function 中有一些小数点来指定优先级；这不是准确的原萃树脂消耗量。
            // 准确的原萃树脂消耗量需要取整“刷取/合成”次数后，乘以对应的树脂消耗量再求和。
            Console.WriteLine($"所需原萃树脂（近似）：{solver.OptimumValue}");
            solver.VariableList
                .Where(x => x.Name != "stock")
                .Where(x => Math.Round(x.OptimizerValue, 2) != 0)
                .ToList()
                .ForEach(x => Console.WriteLine($"\t{x.Name} => 刷取/合成 {Math.Round(x.OptimizerValue, ToZero)} 次"));
        }

        private static double DomainCost(int level)
        {
            // 优先选择高等级的秘境，所以高等级的秘境的消耗较少。
            return level switch
            {
                0 => 20.03,
                1 => 20.02,
                2 => 20.01,
                3 => 20.00,
                _ => throw new ArgumentException($"{level} can only be 0, 1, 2 or 3.")
            };
        }
    }
}