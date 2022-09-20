// Copyright(c) 2022 Sebastian Veith

#pragma once

#include "pch.h"

//#include "config.h"
//#include "log.h"

namespace utility
{
    //    class KeyboardInput
    //    {
    //      public:
    //        bool Init();
    //        bool GetKeyState(Cfg key, bool& isRepeat);
    //
    //      private:
    //        bool UpdateKeyState(const std::set<int>& vkKeySet, bool& isRepeat);
    //
    //        std::map<Cfg, std::set<int>> m_ShortCuts;
    //        std::map<std::set<int>, std::pair<bool, std::chrono::steady_clock::time_point>> m_KeyStates;
    //        const std::chrono::milliseconds m_KeyRepeatDelay = 300ms;
    //    };

    template <typename Sample>
    class Cache
    {

    public:
        Cache(XrTime tolerance, Sample fallback) : m_Tolerance(tolerance), m_Fallback(fallback) {};

        void AddSample(XrTime time, Sample sample)
        {
            m_Cache.insert({ time, sample });
        }

        Sample GetSample(XrTime time) const
        {
            auto it = m_Cache.lower_bound(time);
            bool itIsEnd = m_Cache.end() == it;
            if (!itIsEnd)
            {
                if (it->first == time)
                {
                    // exact entry found
                    return it->second;
                }
                else if (it->first <= time + m_Tolerance)
                {
                    // succeeding entry is within tolerance
                    return it->second;
                }
            }
            bool itIsBegin = m_Cache.begin() == it;
            if (!itIsBegin)
            {
                auto lowerIt = it;
                lowerIt--;
                if (lowerIt->first >= time - m_Tolerance)
                {
                    // preceding entry is within tolerance
                    return lowerIt->second;
                }
            }

            if (!itIsEnd)
            {
                if (!itIsBegin)
                {
                    auto lowerIt = it;
                    lowerIt--;
                    // both etries are valid -> select better match
                    auto returnIt = (time - lowerIt->first < it->first - time ? lowerIt : it);

                    return returnIt->second;
                }
                // higher entry is first in cache -> use it

                return it->second;
            }
            if (!itIsBegin)
            {
                auto lowerIt = it;
                lowerIt--;
                // lower entry is last in cache-> use it
                return lowerIt->second;
            }
            // cache is emtpy -> return fallback
            return m_Fallback;
        }

        // remove outdated entries
        void CleanUp(XrTime time)
        {
            auto it = m_Cache.lower_bound(time - m_Tolerance);
            if (m_Cache.end() != it && m_Cache.begin() != it)
            {
                m_Cache.erase(m_Cache.begin(), it);
            }
        }

        bool empty()
        {
            return m_Cache.empty();
        }

    private:
        std::map<XrTime, Sample> m_Cache{};
        Sample m_Fallback;
        XrTime m_Tolerance;
    };

    //    class Mmf
    //    {
    //      public:
    //        ~Mmf();
    //        void SetName(const std::string& name);
    //        bool Open();
    //        bool Read(void* buffer, size_t size);
    //        void Close();
    //
    //
    //      private: 
    //        std::string m_Name;
    //        HANDLE m_FileHandle{nullptr};
    //        void* m_View{nullptr};
    //    };
    //
    std::string LastErrorMsg(DWORD error);

} // namespace utility