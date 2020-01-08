using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace AoC._2019._23
{
    public class SendOutputEventArgs : EventArgs
    {
        public int Recipient { get; set; }
        public double X { get; set; }
        public double Y { get; set; }
    }

    class IntCode
    {
        public event EventHandler<SendOutputEventArgs> SendOutput;

        private List<double> Instructions { get; set; }
        public int Name { get; set; }
        private bool ReadFromQueue { get; set; }
        private int Pointer { get; set; }
        private int RelativeBase { get; set; }
        private Queue<double> InputQueue { get; set; }
        private List<double> OutputQueue { get; set; }
        public int IdleCount { get; set; }

        public bool IsIdle()
        {
            return InputQueue.Count == 0 && IdleCount > 100;
        }

        public IntCode(List<double> instructions, int name)
        {
            Instructions = instructions;
            Name = name;
            Pointer = 0;
            RelativeBase = 0;
            InputQueue = new Queue<double>();
            OutputQueue = new List<double>();
        }

        public void SendData(double x, double y)
        {
            InputQueue.Enqueue(x);
            InputQueue.Enqueue(y);
        }

        public double? RunProgram()
        {
            double op;
            while (true)
            {
                op = Instructions[Pointer];
                if (op == 99)
                {
                    return null;
                }

                DoOperation(op);
            }
        }

        public void RunOneStep()
        {
            var op = Instructions[Pointer];
            if (op == 99)
            {
                return;
            }

            DoOperation(op);
        }

        public void DoOperation(double operation)
        {
            var opWithZeros = operation.ToString("00000");
            var mode1 = opWithZeros.Substring(2, 1);
            var mode2 = opWithZeros.Substring(1, 1);
            var mode3 = opWithZeros.Substring(0, 1);

            switch (opWithZeros.Substring(3))
            {
                case "01":
                    InsertValue(GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1) + GetValue(Instructions.ElementAtOrDefault(Pointer + 2), mode2), Pointer + 3, mode3);
                    Pointer += 4;
                    break;
                case "02":
                    InsertValue(GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1) * GetValue(Instructions.ElementAtOrDefault(Pointer + 2), mode2), Pointer + 3, mode3);
                    Pointer += 4;
                    break;
                case "03":
                    double input = 0;
                    if (ReadFromQueue)
                    {
                        if (InputQueue.Count == 0)
                        {
                            input = -1;
                            IdleCount++;
                        }
                        else
                        {
                            input = InputQueue.Dequeue();
                            IdleCount = 0;
                        }
                    }
                    else
                    {
                        input = Name;
                        ReadFromQueue = true;
                    }

                    InsertValue(input, Pointer + 1, mode1);
                    Pointer += 2;
                    break;
                case "04":
                    var returnValue = GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1);
                    Pointer += 2;
                    OutputQueue.Add(returnValue);
                    if (OutputQueue.Count == 3)
                    {
                        var args = new SendOutputEventArgs();
                        args.Recipient = (int)OutputQueue[0];
                        args.X = OutputQueue[1];
                        args.Y = OutputQueue[2];
                        OnSendOutput(args);
                    }
                    IdleCount = 0;
                    break;
                case "05":
                    Pointer = GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1) != 0 ? (int)GetValue(Instructions.ElementAtOrDefault(Pointer + 2), mode2) : Pointer + 3;
                    break;
                case "06":
                    Pointer = GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1) == 0 ? (int)GetValue(Instructions.ElementAtOrDefault(Pointer + 2), mode2) : Pointer + 3;
                    break;
                case "07":
                    InsertValue(GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1) < GetValue(Instructions.ElementAtOrDefault(Pointer + 2), mode2) ? 1 : 0, Pointer + 3, mode3);
                    Pointer += 4;
                    break;
                case "08":
                    InsertValue(GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1) == GetValue(Instructions.ElementAtOrDefault(Pointer + 2), mode2) ? 1 : 0, Pointer + 3, mode3);
                    Pointer += 4;
                    break;
                case "09":
                    RelativeBase += (int)GetValue(Instructions.ElementAtOrDefault(Pointer + 1), mode1);
                    Pointer += 2;
                    break;
                default:
                    Console.WriteLine("Bug");
                    break;
            }
        }

        protected virtual void OnSendOutput(SendOutputEventArgs args)
        {
            EventHandler<SendOutputEventArgs> handler = SendOutput;
            handler(this, args);
            OutputQueue.Clear();
        }

        private double GetValue(double value, string mode)
        {
            switch (mode)
            {
                case "0":
                    return Instructions.ElementAtOrDefault((int)value);
                case "1":
                    return value;
                case "2":
                    return Instructions.ElementAtOrDefault((int)(RelativeBase + value));
                default:
                    Console.WriteLine("Bug");
                    return -1;
            }
        }

        private void InsertValue(double value, int position, string mode)
        {
            int positionValue = 0;

            switch (mode)
            {
                case "0":
                    positionValue = (int)Instructions.ElementAtOrDefault(position);
                    break;
                case "2":
                    positionValue = (int)(RelativeBase + Instructions.ElementAtOrDefault(position));
                    break;
                default:
                    Console.WriteLine("Bug");
                    return;
            }



            if (positionValue >= Instructions.Count)
            {
                var itemsToInsert = positionValue - Instructions.Count + 1;

                for (var i = 0; i < itemsToInsert; i++)
                {
                    Instructions.Add(0);
                }
            }

            Instructions[positionValue] = value;
        }
    }

    class Program
    {
        static IntCode[] computers;
        static (double x, double y) natValue;

        static void Main(string[] args)
        {
            computers = new IntCode[50];

            for (var i = 0; i < 50; i++)
            {
                var computer = new IntCode(GetInputFromFile(), i);
                computer.SendOutput += SendOutput;
                computers[i] = computer;
            }

            double latestNatMessage = 0;
            var computerToRun = 0;
            while (true)
            {
                var comp = computers[computerToRun];
                comp.RunOneStep();
                computerToRun = (computerToRun + 1) % 50;

                if (computers.All(c => c.IsIdle()))
                {
                    if (latestNatMessage == natValue.y)
                    {
                        Console.WriteLine(natValue.y);
                    }
                    latestNatMessage = natValue.y;
                    computers[0].SendData(natValue.x, natValue.y);
                }
            }

            static void SendOutput(object sender, SendOutputEventArgs e)
            {
                if (e.Recipient == 255)
                {
                    natValue = (e.X, e.Y);
                }
                else
                {
                    var computerToSendTo = computers[e.Recipient];
                    computerToSendTo.SendData(e.X, e.Y);
                }

            }
        }

        static List<double> GetInputFromFile()
        {
            var data = File.ReadAllText("input.txt");
            return data.Split(',').Select(d => double.Parse(d)).ToList();
        }
    }
}
