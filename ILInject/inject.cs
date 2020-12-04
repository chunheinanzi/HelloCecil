using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Mono.Cecil;
using Mono.Cecil.Cil;

using HelloLog;

namespace ILInject
{
    public class Inject
    {


        public static void InjectIntoCtor(AssemblyDefinition assembiy, String class_name)
        {
            if (class_name.Equals(""))
            {

                return;
            }

            var method = assembiy.MainModule
             .Types.FirstOrDefault(t => t.Name == class_name)
             .Methods.FirstOrDefault(m => m.Name == ".ctor");

            var worker = method.Body.GetILProcessor(); //Get IL

            var Constructor = assembiy.MainModule.ImportReference(typeof(Register).GetConstructor(new Type[] { }));//Create Instance
            var ins = method.Body.Instructions[0];//Get First IL Step 


            ins = method.Body.Instructions[method.Body.Instructions.Count - 1];//Get First IL Step 
            worker.InsertBefore(ins, worker.Create(OpCodes.Newobj, Constructor));
            worker.InsertBefore(ins, worker.Create(OpCodes.Ldarg_0));
            worker.InsertBefore(ins, worker.Create(OpCodes.Call,
                assembiy.MainModule.ImportReference(typeof(Register).GetMethod("RegisterUser"))));////Call Instance Method

            return;


        }

        public static String InjectIntoCSharp(String srcpath, String dstpath)
        {
            if (srcpath.Equals("") || dstpath.Equals(""))
            {
                return "ERROR : Check the Params !\r\n";
            }
            String out_str = "";
            AssemblyDefinition assembiy = AssemblyDefinition.ReadAssembly(srcpath); //Path: dll or exe Path

            foreach (TypeDefinition type in assembiy.MainModule.Types)
            {
                out_str += (string.Format("Class NameSpace :[{0}]\r\n", type.Namespace)); //命名空间
                out_str += (string.Format("Class Name :[{0}]\r\n", type.Name)); //类名


                foreach (FieldDefinition field in type.Fields)
                {
                    out_str += (string.Format("field Name ：[{0}]\r\n", field.FullName));
                }
                if (!type.Name.Equals("<Module>"))
                {
                    InjectIntoCtor(assembiy, type.Name); //在构造函数中插码，
                }

                foreach (MethodDefinition meth in type.Methods) //遍历方法名称
                {

                    out_str += (string.Format("Method Name ：[{0}]\r\n", meth.FullName));

                    out_str += (string.Format(".maxstack {0}\r\n", meth.Body.MaxStackSize));

                    foreach (Instruction inst in meth.Body.Instructions)
                    {
                        out_str += (string.Format("L_{0}: {1} {2}\r\n", inst.Offset.ToString("x4"),
                        inst.OpCode.Name,
                        inst.Operand is String ? String.Format("\"{0}\"", inst.Operand) : inst.Operand));
                    }
                }

            }


            assembiy.Write(dstpath);
            return out_str;

        }
    }
}
