using System;
using System.Collections.Generic;
using System.Linq;

namespace GeneticLocalSearch
{
    class Program
    {
        private static bool step = false;
        static void Main(string[] args)
        {
            Console.WriteLine("Hello Genetic Local Search Algorithm!");
            Console.WriteLine();


            //testGenerateChild(
            //    new Person(new int[] { 10, 7, 4, 9, 8, 6, 2, 5, 3, 1 }),
            //    new Person(new int[] { 9, 8, 6, 3, 5, 7, 10, 4, 1, 2 })
            //    );
            //Console.WriteLine();
            //testGenerateChild(
            //    new Person(new int[] { 5, 10, 1, 3, 6, 2, 4, 7, 8, 9 }),
            //    new Person(new int[] { 4, 7, 2, 1, 3, 9, 10, 8, 5, 6 })
            //    );


            //List<Person> pop = generateTestPopulation();
            //printPopulation(pop);

            Person bestPerson = searchGenetic(generateRandomPopulation(),10);



            Console.WriteLine();
            Console.WriteLine("best is: " + bestPerson);

            
            Console.WriteLine("Press any key to exit..");
            Console.Read();
        }

        static Person searchGenetic(List<Person> population)
        {
            List<Person> pop = population.OrderByDescending(p => p.Grad).ToList();
            Console.WriteLine("Current Population: ");
            printPopulation(pop);
            Person bestPerson = pop[0];
            int count = 0;
            while (bestPerson.Grad != double.MaxValue && count < 300)
            {
                count++;
                if (step)
                {
                    Console.WriteLine("Press any key to continue..");
                    Console.Read();
                }
                Console.WriteLine("Generate childs: ");
                //select best 4

                Person c1 = reproduce(pop);
                Person c2 = reproduce(pop);

                if(pop.FirstOrDefault(p => p.First5Sum == c1.First5Sum && p.Last5Mult == c1.Last5Mult) == null)
                {
                    pop.Add(c1);
                }
                if (pop.FirstOrDefault(p => p.First5Sum == c2.First5Sum && p.Last5Mult == c2.Last5Mult) == null)
                {
                    pop.Add(c2);
                }


                pop = pop.OrderByDescending(p => p.Grad).Take(5).ToList();

                Console.WriteLine("Current Population: count=" + count);
                printPopulation(pop);
                Console.WriteLine();

                bestPerson = pop[0];
            }
            
            
            return bestPerson;
           
        }

        static Person searchGenetic(List<Person> population , int restartCount)
        {
            List<Person> foundList = new List<Person>();
            int count = 0;
            for (int i = 0; i < restartCount; i++)
            {
                count++;
                
                Person f = searchGenetic(generateRandomPopulation());
                Console.WriteLine("retry: " + count);
                if (f.Grad == double.MaxValue)
                {
                    return f;
                }

                foundList.Add(f);
            }
            return foundList.OrderBy(p => p.Grad).Last();
        }

            private static Person reproduce(List<Person> population)
        {
           
            Person a = select(population);
            Person b = select(population.Where(p => p.First5Sum != a.First5Sum && p.Last5Mult != a.Last5Mult).ToList());
            int[] swapIxes;
            Person c = new Person(generateChild(a.Data, b.Data, out swapIxes));
            printChildGenerating(a, b, c, swapIxes);
            return c;
        }

        private static Person select(List<Person> population)
        {
            List<Person> ordered = population.OrderBy(p => p.Grad).ToList();
            double best = population.First().Grad;
            List<object[]> withProb = ordered.Select(p => new object[] {p, p.Grad/best}).ToList();

            Random r = new Random();
            double d = r.NextDouble();
            object[] selected = (object[])withProb.SkipWhile(x => (double)x[1] < d).First();

            return (Person)selected[0];
        }

        private static List<Person> generateRandomPopulation()
        {
            List<Person> pop = new List<Person>();
            for (int i = 0; i < 5; i++)
            {
                List<int> data = new List<int>();
                Random r = new Random();
                while (data.Count < 10)
                {
                    int v = r.Next(10) + 1;
                    if(!data.Contains(v))
                    {
                        data.Add(v);
                    }
                }
                pop.Add(new Person(data.ToArray()));
            }


            return pop;
        }

        private static List<Person> generateTestPopulation()
        {
            List<Person> pop = new List<Person>();
            
            pop.Add(new Person(new int[] { 10, 7, 4, 9, 8, 6, 2, 5, 3, 1 }));
            pop.Add(new Person(new int[] { 4, 7, 2, 1, 3, 9, 10, 8, 5, 6 }));
            pop.Add(new Person(new int[] { 9, 8, 6, 3, 5, 7, 10, 4, 1, 2 }));
            pop.Add(new Person(new int[] { 5, 10, 1, 3, 6, 2, 4, 7, 8, 9 }));
            pop.Add(new Person(new int[] { 5, 2, 1, 9, 8, 7, 6, 4, 10, 3 }));

            return pop;
        }

        private static  void testGenerateChild(Person a, Person b)
        {
            int[] swapIxes;
            Person c = new Person(generateChild(a.Data, b.Data, out swapIxes));
            printChildGenerating(a, b, c, swapIxes);

        }
        private static void printChildGenerating(Person a, Person b, Person c, int[] swapIx)
        {
            string mutationStr = "";
            if (swapIx[0] != -1)
            {
                mutationStr = string.Format("Swap {0}<=>{1} ", swapIx[0], swapIx[1]);
            }
            Console.WriteLine(a);
            Console.WriteLine("+");
            Console.WriteLine(b);
            Console.WriteLine(mutationStr+"=");
            Console.WriteLine(c);
        }

        private static void printPopulation(List<Person> pop)
        {
            foreach (var item in pop)
            {
                Console.WriteLine(item);
            }
        }
        

        private static int sumFirst5(int[] person)
        {
            int sum = 0;
            for (int i = 0; i < 5; i++)
            {
                sum += person[i];
            }
            return sum;
        }
        private static int multLast5(int[] person)
        {
            int mult = 1;
            int[] data = person.ToArray();
            Array.Reverse(data);
            for (int i = 0; i < 5; i++)
            {
                mult = mult * data[i]; 
            }
            return mult;
        }

        private static double calcGrade(int f1, int f2)
        {
            double d = (double)Math.Abs(f1 - 36) / 36 + (double)Math.Abs(f2 - 360) / 360;
            if (d == 0)
            {
                return double.MaxValue;
            }
            return 1 / d;
        }


        private static int[] generateChild(int[] a, int[] b, out int[] swapIxes)
        {
            int[] childData = new int[10];
            for (int i = 0; i < childData.Length; i++)
            {
                childData[i] = -1;
            }

            int[] tempChildData = new int[20];
            for (int i = 0; i < tempChildData.Length; i++)
            {
                tempChildData[i] = -1;
            }

            //merge parents
            for (int i = 0; i < 10; i++)
            {
                tempChildData[2 * i] = a[i];
                tempChildData[(2 * i) + 1] = b[i];
            }

            //clean  duplicate
            int ix = 0;
            for (int i = 0; i < 20; i++)
            {
                int v = tempChildData[i];
                List<int> t = new List<int>(childData);
                if (!childData.Contains(v))
                {
                    childData[ix++] = v;
                }
                if (ix == 10)
                {
                    break;
                }
            }
            swapIxes = new int[] { -1, -1};
            //mutation with chance of 25%
            Random r = new Random();
            if (r.Next(4) == 0)
            {
                swapIxes[0] = r.Next(5);
                swapIxes[1] = r.Next(5) + 5;
                int temp = childData[swapIxes[0]];
                childData[swapIxes[0]] = childData[swapIxes[1]];
                childData[swapIxes[1]] = temp;
            }
            return childData;
        }

        class Person
        {
            public int[] Data { get; set; }
            public int First5Sum { get; set; }
            public int Last5Mult { get; set; }
            public double Grad { get; set; }

            public Person(int[] data)
            {

                Data = data;
                First5Sum = sumFirst5(data);
                Last5Mult = multLast5(data);
                Grad = calcGrade(First5Sum, Last5Mult);
            }

            public override string ToString()
            {
                return string.Format("{0} :: f({1}, {2}) = {3}", string.Join(", ", Data), First5Sum, Last5Mult, Grad);
            }



        }
      
        
    }
}
