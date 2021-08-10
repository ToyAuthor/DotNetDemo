// 這是主要 DLL 檔案。

#include "stdafx.h"
#include <string>

#ifndef WIN32_LEAN_AND_MEAN
#define WIN32_LEAN_AND_MEAN
#endif
#include <windows.h>

#include "CppImage.h"

static char Utf8_Buffer[256];
static wchar_t Wide_Buffer[256];

static char* WideToUtf8( const wchar_t* str )
{
	int size = WideCharToMultiByte( CP_UTF8, 0, str, -1, nullptr, 0, nullptr, false );

	if ( size >= 256 )
	{
		Utf8_Buffer[255] = '\0';
		return Utf8_Buffer;
	}

	WideCharToMultiByte( CP_UTF8, 0, str, -1, Utf8_Buffer, size, nullptr, false );

	Utf8_Buffer[size] = '\0';

	return Utf8_Buffer;
}

static wchar_t* Utf8ToWide( const char* str )
{
	int size = MultiByteToWideChar( CP_UTF8, 0, str, -1, nullptr, 0 );

	if ( size >= 256 )
	{
		Wide_Buffer[255] = L'\0';
		return Wide_Buffer;
	}

	MultiByteToWideChar( CP_UTF8, 0, str, -1, Wide_Buffer, size );

	Wide_Buffer[size] = L'\0';

	return Wide_Buffer;
}

extern "C" __declspec(dllexport) const wchar_t* __stdcall cppimage_tset_string( const wchar_t *str )
{
	static std::wstring  buffer;

	buffer = str;
	buffer += L"_還可以喔";

	return buffer.c_str();
}

using namespace CppImage;

Class1::Class1()
{
	;
}
