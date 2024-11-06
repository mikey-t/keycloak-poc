﻿import Box from '@mui/material/Box'
import Typography from '@mui/material/Typography'
import Link from '@mui/material/Link'

export default function Copyright() {
  return (
    <Box sx={{ pt: '1rem' }}>
      <Typography variant="body2" color="text.secondary" align="center">
        {'Copyright © '}
        <Link color="inherit" href="https://www.youtube.com/watch?v=dQw4w9WgXcQ" target="_blank" rel="noopener">
          John Doe
        </Link>{' '}
        {new Date().getFullYear()}
      </Typography>
    </Box>
  )
}
