extern "C" {
    const char* UMTZ_GetTimeZone() {
        NSTimeZone *timeZone = [NSTimeZone localTimeZone];
        return strdup([[timeZone name] UTF8String]);
    }
}
