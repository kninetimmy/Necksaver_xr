// Copyright(c) 2022 Sebastian Veith

#pragma once

#include "pch.h"
#include <mutex>
#include <map>

namespace utility
{
    template <typename Sample>
    class Cache
    {

    private:
        std::map<XrTime, Sample> m_Cache{};
        Sample m_Fallback;
        XrTime m_Tolerance;
        mutable std::mutex m_Mutex;


    public:
        Cache(XrTime tolerance, Sample fallback) : m_Tolerance(tolerance), m_Fallback(fallback) {};

        void AddSample(XrTime time, Sample sample)
        {
            std::unique_lock lock(m_Mutex);
            m_Cache.insert({ time, sample });
        }

        Sample GetSample(XrTime time) const
        {
            std::unique_lock lock(m_Mutex);

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
            std::unique_lock lock(m_Mutex);
            auto it = m_Cache.lower_bound(time - m_Tolerance);
            if (m_Cache.end() != it && m_Cache.begin() != it)
            {
                m_Cache.erase(m_Cache.begin(), it);
            }
        }

        bool empty()
        {
            std::unique_lock lock(m_Mutex);
            return m_Cache.empty();
        }

    };

} // namespace utility