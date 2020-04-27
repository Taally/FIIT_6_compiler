using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SimpleLang{
    static class ThreeAddressCodeRemoveNoop
   {
       public static Tuple<bool, List<Instruction>> RemoveEmptyNodes(List<Instruction> commands)
       {
           var result = new List<Instruction>();
           var changed = false;
           // three cases:
           // a) Command = noop without label, in this case just remove it
           // b) Command = noop with label, next command - op without label.
           //    Then just combine current label and next op.
           // c) Command = noop with label, in this case,
           //    rename all GOTO current_label to GOTO next_label
           for (var i = 0; i < commands.Count; i++)
           {
               var currentCommand = commands[i];
               if (currentCommand.Operation == "noop" && currentCommand.Label == "") {
                   changed = true;
               }
               // we have label here
               else if (currentCommand.Operation == "noop")
               {
                   // noop is last, just remove it
                   if (i == commands.Count - 1)
                   {
                       changed = true;
                   } 
                   // if next label is empty, we concat current label with next op
                   else if (commands[i + 1].Label == "") {
                       var nextCommand = commands[i + 1];   
                       changed = true;
                       result.Add(
                           new Instruction(
                               currentCommand.Label,
                               nextCommand.Operation,
                               nextCommand.Argument1,
                               nextCommand.Argument2,
                               nextCommand.Result
                           )
                       );
                       i += 1;
                   }
                   // if next label is not empty, instead of noop + next,
                   // rename GOTO current_label to GOTO next_label
                   else
                   {
                       var nextCommand = commands[i + 1];   
                       changed = true;
                       var currentLabel = currentCommand.Label;
                       var nextLabel = nextCommand.Label;

                       result = result
                           .Select(com =>
                               com.Operation == "goto" && com.Argument1 == currentLabel
                                   ? new Instruction(com.Label, com.Operation, nextLabel, com.Argument2, com.Result)
                                   : com
                           ).ToList();

                       for (var j = i + 1; j < commands.Count; j++)
                       {
                           commands[j] = commands[j].Operation == "goto" && commands[j].Argument1 == currentLabel
                               ? new Instruction(
                                   commands[j].Label,
                                   commands[j].Operation,
                                   nextLabel,
                                   commands[j].Argument2,
                                   commands[j].Result
                               )
                               : commands[j];
                       }
                   }
               }
               else
               {
                   result.Add(commands[i]);
               }
           }

           return new Tuple<bool, List<Instruction>>(changed, result);
       }
   }
}
