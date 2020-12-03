using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;
using UnityEngine;
using HelloLog;
/*1.通过Cecil可以完成插码 done
 2.自定义代码封装成DLL，与插码后程序放在一起执行  done
 3.可以通过反射获取类中字段（public）并修改值 done
 4.通过插码遍历所有类，并完成字段打印（使用Cecil）
    4.1 遍历所有类
    4.2 插码
 5.使用Unity程序测试可行性
 */
 /*插码程序*/
 
namespace HelloCecil

{
    public class LogDateTime
    {
        public static void LogDT()
        {
            Console.WriteLine(DateTime.Now.ToString());
        }

        public void init()
        {
            LogDateTime_NonStatic log = new LogDateTime_NonStatic();

            log.LogDT_Field(this);
        }
    }
    
    public class Inject
    {

        public static bool InjectIntoCSharp(String srcpath,String dstpath)
        {
            if (srcpath.Equals("") || dstpath.Equals(""))
            {
                return false;
            }

            AssemblyDefinition assembiy = AssemblyDefinition.ReadAssembly(srcpath); //Path: dll or exe Path

            foreach (TypeDefinition type in assembiy.MainModule.Types)
            {
                Console.WriteLine(string.Format("Class NameSpace :[{0}]", type.Namespace)); //命名空间
                Console.WriteLine(string.Format("Class Name :[{0}]", type.Name)); //类名

                foreach (MethodDefinition meth in type.Methods) //遍历方法名称
                  {
                    
                        Console.WriteLine(string.Format(".maxstack {0}",meth.Body.MaxStackSize));

                        foreach (Instruction inst in meth.Body.Instructions)
                        {
                              Console.WriteLine(string.Format("L_{0}: {1} {2}", inst.Offset.ToString("x4"),
                              inst.OpCode.Name,
                              inst.Operand is String ? String.Format("\"{0}\"", inst.Operand) : inst.Operand));
                        }
                    }
                
            }


            var method = assembiy.MainModule
              .Types.FirstOrDefault(t => t.Name == "MainUIController")
              .Methods.FirstOrDefault(m => m.Name == ".ctor");

            var worker = method.Body.GetILProcessor(); //Get IL

            var Constructor = assembiy.MainModule.ImportReference(typeof(LogDateTime_NonStatic).GetConstructor(new Type[] { }));//Create Instance
            var ins = method.Body.Instructions[0];//Get First IL Step 


            ins = method.Body.Instructions[method.Body.Instructions.Count - 1];//Get First IL Step 
            worker.InsertBefore(ins, worker.Create(OpCodes.Newobj, Constructor));
            worker.InsertBefore(ins, worker.Create(OpCodes.Ldarg_0));
            worker.InsertBefore(ins, worker.Create(OpCodes.Call,
                assembiy.MainModule.ImportReference(typeof(LogDateTime_NonStatic).GetMethod("LogDT_Field"))));////Call Instance Method

           
            assembiy.Write(dstpath);
            return true;

        }
    }

    class Program
    {
        static void Main(string[] args)
        {

            String Path = "D:\\cyou-inc\\TestDevelopment\\menorychange\\Snake\\Snake\\snake_d\\assets\\bin\\Data\\Managed\\";


            bool ret = Inject.InjectIntoCSharp(Path + "Assembly-CSharp.dll", Path + "Assembly-CSharp1.dll");
            if (ret == true)
            {
                Console.WriteLine("Inject OK!");
            }
            else
            {
                Console.WriteLine("Inject error!");
            }


            Console.ReadKey(true);


        }
    }
}
