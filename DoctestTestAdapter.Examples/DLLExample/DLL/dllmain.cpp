// dllmain.cpp : Defines the entry point for the DLL application.
#define DOCTEST_CONFIG_IMPLEMENTATION_IN_DLL
#define DOCTEST_CONFIG_IMPLEMENT
#include <doctest.h>

// Want to keep all warnings enabled but seeing "function not inlined" for TEST_CASE_FIXTURE code 
// so ignoring this specific warning for the examples
#pragma warning(push)
#pragma warning(disable : 4710)
#include "TestIsEvenDLL.h"
#pragma warning(pop) 

#define UNUSED(x) (void)x

BOOL APIENTRY DllMain( HMODULE hModule,
                       DWORD  ul_reason_for_call,
                       LPVOID lpReserved
                     )
{
    UNUSED(hModule);
    UNUSED(lpReserved);

    switch (ul_reason_for_call)
    {
    case DLL_PROCESS_ATTACH:
    case DLL_THREAD_ATTACH:
    case DLL_THREAD_DETACH:
    case DLL_PROCESS_DETACH:
        break;
    }
    return TRUE;
}

