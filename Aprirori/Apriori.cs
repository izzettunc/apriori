using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aprirori
{
    class Apriori
    {
        private List<string> labels;
        private string[][] items;
        public  string output;
        private Dictionary<string, string> dataTable;
        private List<string>[] columnData;
        float minimumSupport;
        float measur_val;
        string measur_type;

        struct rule
        {
            public string text;
            public float support;
        }

        public Apriori(float minSupport,float measurmentValue,string measurment)
        {
            labels = new List<string>();
            dataTable = new Dictionary<string, string>();
            minimumSupport = minSupport;
            measur_val = measurmentValue;
            measur_type = measurment;
            output += "Minimum Support : " + minimumSupport+"\n";
            output += "Minimum " + measur_type + " : " + measur_val + "\n";
            output += "#########################################################################################\n";
            output += "################################   Frequent Item Sets   #################################\n";
            output += "#########################################################################################\n";
        }

        private static string SortString(string input)
        {
            char[] array = input.ToArray();
            Array.Sort(array);
            return new string(array);
        }

        /* In order to make algorithm a little bit easy
       * in input section application added some indexes
       * end of the data. This function solves it and 
       * makes a look up table 
       * ex. low3 -> low or A12 -> A or 111 -> 1
       */
        private Dictionary<string, string> getDataTable()
        {
            if (dataTable.Keys.Count == 0)
            {
                foreach (string[] row in items)
                {
                    int counter = 0;
                    foreach (string item in row)
                    {
                        if (!dataTable.ContainsKey(item))
                        {
                            string x = item;
                            int digit;
                            if (counter != 0)
                                digit = (int)Math.Log10(counter) + 1;
                            else
                                digit = 1;
                            x = x.Remove(x.Length - digit);
                            dataTable.Add(item, x);
                        }
                        counter++;
                    }
                }
            }
            return dataTable;
        }

        /* This function creates an array of lists that
         * contains distinct items in a column*/
        private List<string>[] getColumnData()
        {
            if (columnData == null)
            {
                columnData = new List<string>[items[0].Length];
                for (int i = 0; i < columnData.Length; i++) columnData[i] = new List<string>();
                foreach (string[] row in items)
                {
                    int counter = 0;
                    foreach (string item in row)
                    {
                        if (!columnData[counter].Contains(item))
                        {
                            columnData[counter].Add(item);
                        }
                        counter++;
                    }
                }
            }
            return columnData;
        }

        public void takeInput(string path)
        {
            string[] temp = System.IO.File.ReadAllLines(path);
            string[] firstLine = temp[0].Split(',');

            foreach (string item in firstLine) labels.Add(item);

            string[] lines = temp.Skip(1).ToArray();
            this.items = new string[lines.Length][];
            int i = 0;

            foreach (string line in lines)
            {
                string[] tempS = line.Split(',');
                items[i] = new string[tempS.Length];
                items[i] = tempS;
                for (int j = 0; j < tempS.Length; j++)
                    items[i][j] += j;
                i++;
            }
        }

        /*Creates formatted output*/
        private void createOutput(List<rule> rules)
        {
            Dictionary<string, string> dataLookUpTable = getDataTable();// low1,yes3 gibi verilir low , yes e çevirmek için
            List<string>[] columnLookUpTable = getColumnData();//low1 in yani 1.sutunun başığını öğrenmek için
            foreach (rule rule in rules)
            {
                switch (measur_type)
                {
                    case "Lift":
                        float x = getLift(rule);
                        if (x > measur_val)
                        {
                            string validRule = rule.text;
                            string[] firstPart = validRule.Split(new string[] { "->" }, StringSplitOptions.None);
                            string[] secondPart = firstPart[1].Split('@');
                            firstPart = firstPart[0].Split('@');
                            Queue<string> dataLabel = new Queue<string>();
                            for (int i = 0; i < firstPart.Length; i++)
                            {
                                int j = 0;
                                for (j = 0; j < items[0].Length; j++)
                                {
                                    if (columnLookUpTable[j].Contains(firstPart[i]))
                                    {
                                        dataLabel.Enqueue(labels[j]);
                                        break;
                                    }
                                }
                            }
                            for (int i = 0; i < secondPart.Length; i++)
                            {
                                int j = 0;
                                for (j = 0; j < items[0].Length; j++)
                                {
                                    if (columnLookUpTable[j].Contains(secondPart[i]))
                                    {
                                        dataLabel.Enqueue(labels[j]);
                                        break;
                                    }
                                }
                            }
                            for (int i = 0; i < firstPart.Length; i++)
                            {
                                firstPart[i] = dataLookUpTable[firstPart[i]];
                            }
                            for (int i = 0; i < secondPart.Length; i++)
                            {
                                secondPart[i] = dataLookUpTable[secondPart[i]];
                            }
                            validRule = "";
                            for (int i = 0; i < firstPart.Length; i++)
                            {
                                if (i != firstPart.Length - 1)
                                    validRule += dataLabel.Dequeue() + " IS " + firstPart[i] + " AND ";
                                else
                                    validRule += dataLabel.Dequeue() + " IS " + firstPart[i] + " THEN ";
                            }
                            for (int i = 0; i < secondPart.Length; i++)
                            {
                                if (i != secondPart.Length - 1)
                                    validRule += dataLabel.Dequeue() + " IS " + secondPart[i] + " AND ";
                                else
                                    validRule += dataLabel.Dequeue() + " IS " + secondPart[i];
                            }
                            output += string.Format("{1,-20} {2,-25} {0,-175}", "IF " + validRule, "Support: " + rule.support, "Lift: " + x);
                            output += "\n";
                        }
                        break;
                    case "Leverage":
                        float y = getLeverage(rule);
                        if (y > measur_val)
                        {
                            string validRule = rule.text;
                            string[] firstPart = validRule.Split(new string[] { "->" }, StringSplitOptions.None);
                            string[] secondPart = firstPart[1].Split('@');
                            firstPart = firstPart[0].Split('@');
                            Queue<string> dataLabel = new Queue<string>();
                            for (int i = 0; i < firstPart.Length; i++)
                            {
                                int j = 0;
                                for (j = 0; j < items[0].Length; j++)
                                {
                                    if (columnLookUpTable[j].Contains(firstPart[i]))
                                    {
                                        dataLabel.Enqueue(labels[j]);
                                        break;
                                    }
                                }
                            }
                            for (int i = 0; i < secondPart.Length; i++)
                            {
                                int j = 0;
                                for (j = 0; j < items[0].Length; j++)
                                {
                                    if (columnLookUpTable[j].Contains(secondPart[i]))
                                    {
                                        dataLabel.Enqueue(labels[j]);
                                        break;
                                    }
                                }
                            }
                            for (int i = 0; i < firstPart.Length; i++)
                            {
                                firstPart[i] = dataLookUpTable[firstPart[i]];
                            }
                            for (int i = 0; i < secondPart.Length; i++)
                            {
                                secondPart[i] = dataLookUpTable[secondPart[i]];
                            }
                            validRule = "";
                            for (int i = 0; i < firstPart.Length; i++)
                            {
                                if (i != firstPart.Length - 1)
                                    validRule += dataLabel.Dequeue() + " IS " + firstPart[i] + " AND ";
                                else
                                    validRule += dataLabel.Dequeue() + " IS " + firstPart[i] + " THEN ";
                            }
                            for (int i = 0; i < secondPart.Length; i++)
                            {
                                if (i != secondPart.Length - 1)
                                    validRule += dataLabel.Dequeue() + " IS " + secondPart[i] + " AND ";
                                else
                                    validRule += dataLabel.Dequeue() + " IS " + secondPart[i];
                            }
                            output += string.Format("{1,-20} {2,-25} {0,-175}", "IF " + validRule, "Support: " + rule.support, "Leverage: " + y);
                            output += "\n";
                        }
                        break;
                    case "Confidence"://Lift in aynısı sadece Confidence için yapılmışı
                        float z = getConfidence(rule);
                        if (z > measur_val)
                        {
                            string validRule = rule.text;
                            string[] firstPart = validRule.Split(new string[] { "->" }, StringSplitOptions.None);
                            string[] secondPart = firstPart[1].Split('@');
                            firstPart = firstPart[0].Split('@');
                            Queue<string> dataLabel = new Queue<string>();
                            for (int i = 0; i < firstPart.Length; i++)
                            {
                                int j = 0;
                                for (j = 0; j < items[0].Length; j++)
                                {
                                    if (columnLookUpTable[j].Contains(firstPart[i]))
                                    {
                                        dataLabel.Enqueue(labels[j]);
                                        break;
                                    }
                                }
                            }
                            for (int i = 0; i < secondPart.Length; i++)
                            {
                                int j = 0;
                                for (j = 0; j < items[0].Length; j++)
                                {
                                    if (columnLookUpTable[j].Contains(secondPart[i]))
                                    {
                                        dataLabel.Enqueue(labels[j]);
                                        break;
                                    }
                                }
                            }
                            for (int i = 0; i < firstPart.Length; i++)
                            {
                                firstPart[i] = dataLookUpTable[firstPart[i]];
                            }
                            for (int i = 0; i < secondPart.Length; i++)
                            {
                                secondPart[i] = dataLookUpTable[secondPart[i]];
                            }
                            validRule = "";
                            for (int i = 0; i < firstPart.Length; i++)
                            {
                                if (i != firstPart.Length - 1)
                                    validRule += dataLabel.Dequeue() + " IS " + firstPart[i] + " AND ";
                                else
                                    validRule += dataLabel.Dequeue() + " IS " + firstPart[i] + " THEN ";
                            }
                            for (int i = 0; i < secondPart.Length; i++)
                            {
                                if (i != secondPart.Length - 1)
                                    validRule += dataLabel.Dequeue() + " IS " + secondPart[i] + " AND ";
                                else
                                    validRule += dataLabel.Dequeue() + " IS " + secondPart[i];
                            }
                            output += string.Format("{1,-20} {2,-25} {0,-175}", "IF " + validRule, "Support: " + rule.support, "Confidence: " + z);
                            output += "\n";
                        }
                        break;
                }
            }
            output += (rules.Count + " rule found.");
        }

        /*Finds the support of given input*/
        private float getSupport(string input)
        {
            int count = 0;
            string[] wanted = input.Split('@');
            int numberTBFound = wanted.Length;
            foreach (string[] row in items)
            {
                int counter = 0;
                for (int j = 0; j < wanted.Length; j++)
                {
                    foreach (string element in row)
                    {
                        if (wanted[j] == element) counter++;
                    }
                }
                if (counter == numberTBFound) count++;
            }
            return (float)((count * 1.0) / (items.Length * 1.0));
        }

        private float getConfidence(rule input)
        {
            string[] wantedRule = input.text.Split(new string[] { "->" }, StringSplitOptions.None);
            float denominator = getSupport(wantedRule[0]);
            float numerator = input.support;
            return numerator / denominator;
        }

        private float getLeverage(rule input)
        {
            string[] wantedRule = input.text.Split(new string[] { "->" }, StringSplitOptions.None);
            float A = input.support;
            float B = getSupport(wantedRule[0]);
            float C = getSupport(wantedRule[1]);
            return A - (B * C);

        }

        private float getLift(rule input)
        {
            string[] wantedRule = input.text.Split(new string[] { "->" }, StringSplitOptions.None);
            float A = input.support;
            float B = getSupport(wantedRule[0]);
            float C = getSupport(wantedRule[1]);
            return A / (B * C);
        }

        /*Rules form changed to computable way (AB->C to ABC) */
        private float getRuleSupport(rule input)
        {
            string[] wantedRule = input.text.Split(new string[] { "->" }, StringSplitOptions.None);
            string compositeRule = wantedRule[0] + "@" + wantedRule[1];
            return getSupport(compositeRule);
        }

        /* This test basicaly calculate support of given rules
         * if it's bigger than minimum support it becomes frequent rule
         * if it's not then it's deleted.*/
        private List<string> frequencyTest(List<string> input)
        {
            List<string> toBeDeleted = new List<string>();
            for (int i = 0; i < input.Count; i++)
            {
                int count = 0;
                string[] wanted = input[i].Split('@');
                int numberOfTBFound = wanted.Length;
                foreach (string[] row in items)
                {
                    int counter = 0;
                    for (int j = 0; j < wanted.Length; j++)
                    {
                        foreach (string element in row)
                        {
                            if (wanted[j] == element) counter++;
                        }
                    }
                    if (counter == numberOfTBFound) count++;
                }
                if ((count * 1.0) / (items.Length * 1.0) < minimumSupport) toBeDeleted.Add(input[i]);
                else
                {
                    Dictionary<string, string> LookUpTable = getDataTable();
                    string[] frequentItemSet = input[i].Split('@');
                    for (int j = 0; j < frequentItemSet.Length; j++)
                    {
                        output += LookUpTable[frequentItemSet[j]] + " ";
                    }
                    output += "\n";
                }
            }
            foreach (string item in toBeDeleted) input.Remove(item);
            return input;
        }

        /* This function first splits the input ABCD to A,B,C,D
         * and then creates of its multiples up to one less from input length
         * so in that case function creates singles,doubles,triples but not quadruples
         * (A,B,C,D,AB,AC,AD,BC,BD,CD,ABC,ACD,ADB,BCD) -> list of multiples (rule particles)
         * and then matches these rules particles and create every possible distinct rules
         * A->B,A->C,...,AC->B,AC->D,...,ABC->D,...,BCD->A
         */
        private void createRules(List<string> ruleCandidates, int columnCount)
        {
            List<rule> Rules = new List<rule>();
            foreach (string candidate in ruleCandidates)
            {
                List<string> ruleParticles = new List<string>();
                string[] elements = candidate.Split('@');//Splits to singles
                List<string> sets = new List<string>();
                foreach (string element in elements) { sets.Add(element); ruleParticles.Add(element); }//singles added as ruleParticle for becoming input of the next iteration
                List<string> nextSets = new List<string>();
                
                for (int i = 0; i < elements.Length - 2; i++)
                {
                    foreach (string item in sets)//Split to singles and looks for anything can be add
                    {
                        string[] contents = item.Split('@');
                        foreach (string element in elements)
                        {
                            bool found = true;//flag for anything found to be add
                            foreach (string content in contents)
                            {
                                if (element == content) { found = false; break; }
                            }
                            if (found)
                            {
                                string toBeAdded = element + "@" + item;
                                foreach (string particle in ruleParticles)
                                {
                                    if (SortString(particle) == SortString(toBeAdded)) found = false;
                                }
                                if (found)
                                {
                                    ruleParticles.Add(toBeAdded);//When creates multiples(singles,doubles,triples and so on) add them as rule praticle
                                    nextSets.Add(toBeAdded);//its stored her for becoming next iterations input
                                }
                            }
                        }
                    }
                    sets.Clear();
                    foreach (string item in nextSets)
                    {
                        sets.Add(item);
                    }
                    nextSets.Clear();
                }
                for (int i = 0; i < ruleParticles.Count; i++)//Creates all possible rules with rule particles
                {
                    for (int j = i; j < ruleParticles.Count; j++)
                    {
                        bool distinct = true;
                        string[] a = ruleParticles[i].Split('@');
                        string[] b = ruleParticles[j].Split('@');
                        for (int k = 0; k < a.Length; k++)//checks for AB->B situation
                        {
                            for (int l = 0; l < b.Length; l++)
                            {
                                if (a[k] == b[l]) { distinct = false; break; }
                            }
                            if (!distinct) break;
                        }
                        if (distinct)//yoksa i -> j ve tersi olan j->i diye iki kural oluşturulur ve kurallara eklenir
                        {
                            rule newRule = new rule();
                            newRule.text = ruleParticles[i] + "->" + ruleParticles[j];
                            if (!Rules.Contains(newRule))
                                Rules.Add(newRule);
                            rule reverseNewRule = new rule();
                            reverseNewRule.text = ruleParticles[j] + "->" + ruleParticles[i];
                            if (!Rules.Contains(reverseNewRule))
                                Rules.Add(reverseNewRule);
                        }
                    }
                }
                ruleParticles.Clear();
                nextSets.Clear();
                sets.Clear();
            }
            for (int i = 0; i < Rules.Count; i++)
            {
                rule k = Rules[i];
                k.support = getRuleSupport(Rules[i]);//her bir kuralın support u hesaplanır
                Rules[i] = k;
            }
            createOutput(Rules);
        }

        private List<string> getRuleCandidates(int columnCount)
        {
            List<string> results = new List<string>();
            List<string> sets = new List<string>();
            List<string>[] columns = new List<string>[columnCount];
            for (int i = 0; i < columnCount; i++) columns[i] = new List<string>();
            foreach (string[] row in items)
            {
                for (int i = 0; i < columnCount; i++)
                {
                    if (!columns[i].Contains(row[i]))
                    {
                        columns[i].Add(row[i]);
                    }
                }
            }
            //Finds singles
            int wantedColumn = 0;
            foreach (List<string> categories in columns)
            {
                List<string> toBeDeleted = new List<string>();
                for (int i = 0; i < categories.Count; i++)
                {
                    string wanted = categories[i];
                    int count = 0;
                    foreach (string[] row in items)
                    {
                        if (row[wantedColumn] == wanted) count++;
                    }
                    if (count * 1.0 / items.Length * 1.0 < minimumSupport)
                    {
                        toBeDeleted.Add(wanted);
                    }
                }
                foreach (string item in toBeDeleted) categories.Remove(item);
                foreach (string category in categories) sets.Add(category);
                wantedColumn++;
            }
            List<string> toBeChecked = new List<string>();
            while (sets.Count > 0)
            {
                foreach (string set in sets)
                {
                    string[] contents = set.Split('@');
                    for (int i = 0; i < columnCount; i++)
                    {
                        bool distinct = true;
                        foreach (string content in contents)
                        {
                            if (columns[i].Contains(content)) distinct = false;
                        }
                        if (distinct)
                        {
                            foreach (string item in columns[i])
                            {
                                string toBeAdded = set + "@" + item;
                                foreach (string element in toBeChecked)
                                {
                                    string t1 = SortString(element);
                                    string t2 = SortString(toBeAdded);
                                    if (t1 == t2) { distinct = false; break; }
                                }
                                if (distinct) toBeChecked.Add(set + "@" + item);
                            }
                        }
                    }
                }
                List<string> frequentSets = frequencyTest(toBeChecked);
                sets.Clear();
                foreach (string set in frequentSets)
                {
                    sets.Add(set);
                }
                toBeChecked.Clear();
                foreach (string set in sets)
                {
                    results.Add(set);
                }
            }
            output += "#########################################################################################\n";
            output += "#####################################      Rules     ####################################\n";
            output += "#########################################################################################\n";
            return results;
        }

        public string startApriori()
        {
            List<string> ruleCandidates = getRuleCandidates(items[0].Length);
            createRules(ruleCandidates, items[0].Length);
            return output;
        }
    }
}
