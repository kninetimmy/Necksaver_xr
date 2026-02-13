#pragma once

#include <cstdarg>
#include <cstdio>
#include <windows.h>

namespace motion_compensation_layer::log {

inline void ErrorLog(const char* fmt, ...) {
    char buf[1024];
    va_list args;
    va_start(args, fmt);
    _vsnprintf_s(buf, sizeof(buf), _TRUNCATE, fmt, args);
    va_end(args);

    OutputDebugStringA(buf);
    std::fputs(buf, stderr);
}

} // namespace motion_compensation_layer::log
