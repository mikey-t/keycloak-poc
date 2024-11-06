import Button from '@mui/material/Button/Button'
import { useState } from 'react'
import ProtectedApi from '../../logic/ProtectedApi'
import PageTitle from '../../components/PageTitle'
import { Box } from '@mui/material'

const api = new ProtectedApi()

export default function Account() {
  const [data, setData] = useState<string>('')

  // const getData = async () => {
  //   const res = await api.getProtectedTest()
  //   if (res.isError()) {
  //     console.log(res.exception?.toJson())
  //     return
  //   }

  //   setData(res.data || '')
  // }

  const getToken = async () => {
    console.log('attempting to get token')
    const res = await api.getKeycloakToken()
    setData(res.data || '')
  }

  // This isn't how this will actually be initiated - for initial testing, utilize the SSO link directly in the browser
  // const initiateOidcSso = async () => {
  //   console.log('attempting to initiate OIDC SSO')
  //   await api.initiateOidcSso()
  // }

  const clear = () => {
    setData('')
  }

  return (
    <>
      <PageTitle>Account</PageTitle>
      {/* <p>This page requires authentication.</p><Button variant="contained" onClick={getData}>Test protected endpoint</Button>&nbsp;*/}
      <Button variant="contained" onClick={getToken}>Get keycloak token</Button>&nbsp;
      <Button variant="contained" onClick={clear}>Clear</Button>
      <Box sx={{ pb: '30px' }}>{data}</Box>
      {/* <Button variant="contained" onClick={initiateOidcSso}>Initiate OIDC SSO</Button>&nbsp; */}
    </>
  )
}
