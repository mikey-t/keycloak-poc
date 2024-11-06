﻿import { ReactNode, useEffect } from 'react'
import { useLocation } from 'react-router-dom'

// Note that new react-router has component <ScrollRestoration /> (un-tested)
export default function ScrollToTop({ children }: { children: ReactNode }) {
  const { pathname } = useLocation()

  useEffect(() => {
    window.scrollTo(0, 0)
  }, [pathname])

  return children
}
