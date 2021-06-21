using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using OpenPA.Native;
using static OpenPA.Native.pa_operation;
using static OpenPA.Native.pa_context;

namespace OpenPA
{
    public unsafe partial class PAContext
    {
        #region Callbacks
        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static void ModuleCallback(pa_context* ctx, pa_module_info* info, int eol, void* userdata)
        {
            if (eol == 0)
            {
                *((pa_module_info**)userdata) = info;

                MainLoop.Instance.Signal(1);
            }
            else
            {
                *((pa_module_info**)userdata) = null;

                MainLoop.Instance.Signal(0);
            }
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static void LoadModuleCallback(pa_context* ctx, uint index, void* userdata)
        {
            *((uint*)userdata) = index;

            MainLoop.Instance.Signal(0);
        }

        [UnmanagedCallersOnly(CallConvs = new[] { typeof(CallConvCdecl) })]
        static void UnloadModuleCallback(pa_context* ctx, int result, void* userdata)
        {
            *((bool*)userdata) = result == 1;

            MainLoop.Instance.Signal(0);
        }
        #endregion

        /// <summary>
        /// Get some information about a module by it's index
        /// </summary>
        /// <param name="index">Index of module</param>
        /// <returns>Module information</returns>
        public Task<ModuleInfo?> GetModuleInfoAsync(uint index) => Task.Run(() => GetModuleInfo(index));

        /// <summary>
        /// Get some information about a module by it's index
        /// </summary>
        /// <param name="index">Index of module</param>
        /// <returns>Module information</returns>
        public ModuleInfo? GetModuleInfo(uint index)
        {
            // Returned object
            ModuleInfo? info = default;

            // Lock the context so that we can remain thread-safe
            Monitor.Enter(this);

            // Pointer to returned pa_module_info structure
            pa_module_info* module_info = null;

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Call the native get_module_info native function passing in the name and callback
            pa_operation* op = pa_context.pa_context_get_module_info(pa_Context, index, &ModuleCallback, &module_info);

            // Wait for the callback to signal
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && module_info == null)
                MainLoop.Instance.Wait();

            // If the callback returned data
            if (module_info != null)
            {
                // Copy the unmanaged sink_info structure into a ModuleInfo object
                info = ModuleInfo.Convert(*module_info);

                // Allow PulseAudio to free the module_info
                MainLoop.Instance.Accept();

                // Wait for the mainloop to complete
                MainLoop.Instance.Wait();

            }

            // Dereference the pa_operation
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Unlock the context
            Monitor.Exit(this);

            // Return the ModuleInfo object
            return info;

        }

        /// <summary>
        /// Get the complete list of currently loaded modules
        /// </summary>
        /// <returns>Module information list</returns>
        public Task<IReadOnlyList<ModuleInfo?>> GetModuleInfoListAsync() => Task.Run(GetModuleInfoList);

        /// <summary>
        /// Get the complete list of currently loaded modules
        /// </summary>
        /// <returns>Module information list</returns>
        public IReadOnlyList<ModuleInfo?> GetModuleInfoList()
        {
            // Returned list
            List<ModuleInfo?> modules = new();

            // Lock the context so that we can remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Pointer to the returned pa_module_info structure
            pa_module_info* module_info = null;

            // Call the native get_module_info native function passing in the name and callback
            pa_operation* op = pa_context.pa_context_get_module_info_list(pa_Context, &ModuleCallback, &module_info);

            // Loop through list of sinks
            while (true)
            {
                // Wait for the callback to signal
                while (pa_operation_get_state(op) == Enums.OperationState.RUNNING && module_info == null)
                    MainLoop.Instance.Wait();

                // If the callback returned data
                if (module_info != null)
                {
                    // Copy the unmanaged sink_info structure into a ModuleInfo object
                    // and add it to the list
                    modules.Add(ModuleInfo.Convert(*module_info));

                    // Allow PulseAudio to free the pa_module_info structure
                    MainLoop.Instance.Accept();

                    // Reset module_info pointer
                    module_info = null;
                }
                else
                {
                    // Break out of the loop when the list is done
                    break;
                }
            }

            // Dereference the pa_operation
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Unlock the context
            Monitor.Exit(this);

            // Return the ModuleInfo object List
            return modules;

        }

        /// <summary>
        /// Loads a module
        /// </summary>
        /// <param name="name">Module to load</param>
        /// <param name="arguments">Module arguments</param>
        /// <returns>Index of module</returns>
        public Task<uint> LoadModuleAsync(string name, string? arguments) => Task.Run(() => LoadModule(name, arguments));

        /// <summary>
        /// Loads a module
        /// </summary>
        /// <param name="name">Module to load</param>
        /// <param name="arguments">Module arguments</param>
        /// <returns>Index of module</returns>
        public uint LoadModule(string name, string? arguments)
        {
            // Hold lock to remain thread-safe
            Monitor.Enter(this);

            // Copy strings to unmanaged memory
            IntPtr namePtr = Marshal.StringToHGlobalAnsi(name);
            IntPtr argPtr = Marshal.StringToHGlobalAnsi(arguments);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Set up a place to receive the module's index
            uint index = 0;

            // Call the operation
            pa_operation* op = pa_context_load_module(pa_Context, namePtr, argPtr, &LoadModuleCallback, &index);

            // Wait for the operation to complete
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the pa_operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Free the unmanaged strings
            if (argPtr != IntPtr.Zero)
                Marshal.FreeHGlobal(argPtr);

            Marshal.FreeHGlobal(namePtr);

            // Release lock
            Monitor.Exit(this);

            // Return index
            return index;
        }

        /// <summary>
        /// Unloads a module
        /// </summary>
        /// <param name="index">Index of module</param>
        /// <returns>Success</returns>
        public Task<bool> UnloadModuleAsync(uint index) => Task.Run(() => UnloadModule(index));

        /// <summary>
        /// Unloads a module
        /// </summary>
        /// <param name="index">Index of module</param>
        /// <returns>Success</returns>
        public bool UnloadModule(uint index)
        {
            // Hold lock to remain thread-safe
            Monitor.Enter(this);

            // Lock the mainloop
            MainLoop.Instance.Lock();

            // Set up a place to receive the result
            bool success = false;

            // Invoke the operation
            pa_operation* op = pa_context_unload_module(pa_Context, index, &UnloadModuleCallback, &success);

            // Wait until the operation completes
            while (pa_operation_get_state(op) == Enums.OperationState.RUNNING)
                MainLoop.Instance.Wait();

            // Dereference the pa_operation structure
            pa_operation_unref(op);

            // Unlock the mainloop
            MainLoop.Instance.Unlock();

            // Release lock
            Monitor.Exit(this);

            // Return the result
            return success;
        }
    }
}
