// �o�O�D�n DLL �ɮסC

#include "stdafx.h"
#include "CppDoor.h"
#include <cstdint>
#include <cstdio>

extern "C" __declspec(dllexport) void CppDoor001(uint8_t * pArray, int nSize)
{
	for (int i = 0; i<nSize; i++)
		printf("%d\n", pArray[i]);
}