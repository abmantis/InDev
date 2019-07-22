﻿Start-Transcript ($PSScriptRoot + '\Temp\runbuild.txt')

$version = Get-Content "$PSScriptRoot\version.json" | Out-String | ConvertFrom-Json

& $PSScriptRoot\runbuild.ps1 -properties @{"majorVersion"="$($version.Major).0"; "majorWithReleaseVersion"="$($version.Major).0.$($version.Release)"; "nugetPrerelease"=$version.Prerelease; "zipFileName"="Json$($version.Major)0r$($version.Release).zip"; "treatWarningsAsErrors"=$true}
# SIG # Begin signature block
# MIIfuQYJKoZIhvcNAQcCoIIfqjCCH6YCAQExDzANBglghkgBZQMEAgEFADB5Bgor
# BgEEAYI3AgEEoGswaTA0BgorBgEEAYI3AgEeMCYCAwEAAAQQH8w7YFlLCE63JNLG
# KX7zUQIBAAIBAAIBAAIBAAIBADAxMA0GCWCGSAFlAwQCAQUABCDdfFhGVM0Vb831
# hITqWK6gZ5xf7rRav4dsyB+UMBvP/6CCDfMwggPFMIICraADAgECAhACrFwmagtA
# m48LefKuRiV3MA0GCSqGSIb3DQEBBQUAMGwxCzAJBgNVBAYTAlVTMRUwEwYDVQQK
# EwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xKzApBgNV
# BAMTIkRpZ2lDZXJ0IEhpZ2ggQXNzdXJhbmNlIEVWIFJvb3QgQ0EwHhcNMDYxMTEw
# MDAwMDAwWhcNMzExMTEwMDAwMDAwWjBsMQswCQYDVQQGEwJVUzEVMBMGA1UEChMM
# RGlnaUNlcnQgSW5jMRkwFwYDVQQLExB3d3cuZGlnaWNlcnQuY29tMSswKQYDVQQD
# EyJEaWdpQ2VydCBIaWdoIEFzc3VyYW5jZSBFViBSb290IENBMIIBIjANBgkqhkiG
# 9w0BAQEFAAOCAQ8AMIIBCgKCAQEAxszlc+b71LvlLS0ypt/lgT/JzSVJtnEqw9WU
# NGeiChywX2mmQLHEt7KP0JikqUFZOtPclNY823Q4pErMTSWC90qlUxI47vNJbXGR
# fmO2q6Zfw6SE+E9iUb74xezbOJLjBuUIkQzEKEFV+8taiRV+ceg1v01yCT2+OjhQ
# W3cxG42zxyRFmqesbQAUWgS3uhPrUQqYQUEiTmVhh4FBUKZ5XIneGUpX1S7mXRxT
# LH6YzRoGFqRoc9A0BBNcoXHTWnxV215k4TeHMFYE5RG0KYAS8Xk5iKICEXwnZreI
# t3jyygqoOKsKZMK/Zl2VhMGhJR6HXRpQCyASzEG7bgtROLhLywIDAQABo2MwYTAO
# BgNVHQ8BAf8EBAMCAYYwDwYDVR0TAQH/BAUwAwEB/zAdBgNVHQ4EFgQUsT7DaQP4
# v0cB1JgmGggC72NkK8MwHwYDVR0jBBgwFoAUsT7DaQP4v0cB1JgmGggC72NkK8Mw
# DQYJKoZIhvcNAQEFBQADggEBABwaBpfc15yfPIhmBghXIdshR/gqZ6q/GDJ2QBBX
# wYrzetkRZY41+p78RbWe2UwxS7iR6EMsjrN4ztvjU3lx1uUhlAHaVYeaJGT2imbM
# 3pw3zag0sWmbI8ieeCIrcEPjVUcxYRnvWMWFL04w9qAxFiPI5+JlFjPLvxoboD34
# yl6LMYtgCIktDAZcUrfE+QqY0RVfnxK+fDZjOL1EpH/kJisKxJdpDemM4sAQV7jI
# dhKRVfJIadi8KgJbD0TUIDHb9LpwJl2QYJ68SxcJL7TLHkNoyQcnwdJc9+ohuWgS
# nDycv578gFybY83sR6olJ2egN/MAgn1U16n46S4To3foH0owggSRMIIDeaADAgEC
# AhAHsEGNpR4UjDMbvN63E4MjMA0GCSqGSIb3DQEBCwUAMGwxCzAJBgNVBAYTAlVT
# MRUwEwYDVQQKEwxEaWdpQ2VydCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5j
# b20xKzApBgNVBAMTIkRpZ2lDZXJ0IEhpZ2ggQXNzdXJhbmNlIEVWIFJvb3QgQ0Ew
# HhcNMTgwNDI3MTI0MTU5WhcNMjgwNDI3MTI0MTU5WjBaMQswCQYDVQQGEwJVUzEY
# MBYGA1UEChMPLk5FVCBGb3VuZGF0aW9uMTEwLwYDVQQDEyguTkVUIEZvdW5kYXRp
# b24gUHJvamVjdHMgQ29kZSBTaWduaW5nIENBMIIBIjANBgkqhkiG9w0BAQEFAAOC
# AQ8AMIIBCgKCAQEAwQqv4aI0CI20XeYqTTZmyoxsSQgcCBGQnXnufbuDLhAB6GoT
# NB7HuEhNSS8ftV+6yq8GztBzYAJ0lALdBjWypMfL451/84AO5ZiZB3V7MB2uxgWo
# cV1ekDduU9bm1Q48jmR4SVkLItC+oQO/FIA2SBudVZUvYKeCJS5Ri9ibV7La4oo7
# BJChFiP8uR+v3OU33dgm5BBhWmth4oTyq22zCfP3NO6gBWEIPFR5S+KcefUTYmn2
# o7IvhvxzJsMCrNH1bxhwOyMl+DQcdWiVPuJBKDOO/hAKIxBG4i6ryQYBaKdhDgaA
# NSCik0UgZasz8Qgl8n0A73+dISPumD8L/4mdywIDAQABo4IBPzCCATswHQYDVR0O
# BBYEFMtck66Im/5Db1ZQUgJtePys4bFaMB8GA1UdIwQYMBaAFLE+w2kD+L9HAdSY
# JhoIAu9jZCvDMA4GA1UdDwEB/wQEAwIBhjATBgNVHSUEDDAKBggrBgEFBQcDAzAS
# BgNVHRMBAf8ECDAGAQH/AgEAMDQGCCsGAQUFBwEBBCgwJjAkBggrBgEFBQcwAYYY
# aHR0cDovL29jc3AuZGlnaWNlcnQuY29tMEsGA1UdHwREMEIwQKA+oDyGOmh0dHA6
# Ly9jcmwzLmRpZ2ljZXJ0LmNvbS9EaWdpQ2VydEhpZ2hBc3N1cmFuY2VFVlJvb3RD
# QS5jcmwwPQYDVR0gBDYwNDAyBgRVHSAAMCowKAYIKwYBBQUHAgEWHGh0dHBzOi8v
# d3d3LmRpZ2ljZXJ0LmNvbS9DUFMwDQYJKoZIhvcNAQELBQADggEBALNGxKTz6gq6
# clMF01GjC3RmJ/ZAoK1V7rwkqOkY3JDl++v1F4KrFWEzS8MbZsI/p4W31Eketazo
# Nxy23RT0zDsvJrwEC3R+/MRdkB7aTecsYmMeMHgtUrl3xEO3FubnQ0kKEU/HBCTd
# hR14GsQEccQQE6grFVlglrew+FzehWUu3SUQEp9t+iWpX/KfviDWx0H1azilMX15
# lzJUxK7kCzmflrk5jCOCjKqhOdGJoQqstmwP+07qXO18bcCzEC908P+TYkh0z9gV
# rlj7tyW9K9zPVPJZsLRaBp/QjMcH65o9Y1hD1uWtFQYmbEYkT1K9tuXHtQYx1Rpf
# /dC8Nbl4iukwggWRMIIEeaADAgECAhAKcaGwwpb1x5BlRwo8IFN+MA0GCSqGSIb3
# DQEBCwUAMFoxCzAJBgNVBAYTAlVTMRgwFgYDVQQKEw8uTkVUIEZvdW5kYXRpb24x
# MTAvBgNVBAMTKC5ORVQgRm91bmRhdGlvbiBQcm9qZWN0cyBDb2RlIFNpZ25pbmcg
# Q0EwHhcNMTgxMDI1MDAwMDAwWhcNMjExMDI5MTIwMDAwWjCBjDEUMBIGA1UEBRML
# NjAzIDM4OSAwNjgxCzAJBgNVBAYTAlVTMQswCQYDVQQIEwJ3YTEQMA4GA1UEBxMH
# UmVkbW9uZDEjMCEGA1UEChMaSnNvbi5ORVQgKC5ORVQgRm91bmRhdGlvbikxIzAh
# BgNVBAMTGkpzb24uTkVUICguTkVUIEZvdW5kYXRpb24pMIIBIjANBgkqhkiG9w0B
# AQEFAAOCAQ8AMIIBCgKCAQEAuN7uHXlw69xJiDtuvtaEmG2+W4tcwqucLUj7xf+D
# W5mTiqIa4uSUZ1upBEQmpNnjHV6iKvMzDLjMsjNEhgMnkoL+6HBvhy5GOyKpG/T3
# NHxX6bUN3JTExwZUD/CpXHIh31n3KSthNRlXyZod+5hWuC93UJxzNcXtR7iYR39x
# 5oIx4IEdq0xyXt9PclKu/V6R9D7Fjllx9aKlTg/hnPoMKF35P7kxMv5ULagSv3HO
# JkKa70biuqv8625wBE9HNA9S5LXNFMk2ZuUhYTxLk1qYx58me+NU6VUvoDsqzLx+
# GXTAkl6r1kSeDMskOe4fXevoCkPm1BFZHd68MmH6tIuMLQIDAQABo4ICHjCCAhow
# HwYDVR0jBBgwFoAUy1yTroib/kNvVlBSAm14/KzhsVowHQYDVR0OBBYEFF4wRcl3
# CrbuKXpbC4oTKXnW5tsmMDQGA1UdEQQtMCugKQYIKwYBBQUHCAOgHTAbDBlVUy1X
# QVNISU5HVE9OLTYwMyAzODkgMDY4MA4GA1UdDwEB/wQEAwIHgDATBgNVHSUEDDAK
# BggrBgEFBQcDAzCBmQYDVR0fBIGRMIGOMEWgQ6BBhj9odHRwOi8vY3JsMy5kaWdp
# Y2VydC5jb20vTkVURm91bmRhdGlvblByb2plY3RzQ29kZVNpZ25pbmdDQS5jcmww
# RaBDoEGGP2h0dHA6Ly9jcmw0LmRpZ2ljZXJ0LmNvbS9ORVRGb3VuZGF0aW9uUHJv
# amVjdHNDb2RlU2lnbmluZ0NBLmNybDBMBgNVHSAERTBDMDcGCWCGSAGG/WwDATAq
# MCgGCCsGAQUFBwIBFhxodHRwczovL3d3dy5kaWdpY2VydC5jb20vQ1BTMAgGBmeB
# DAEEATCBhAYIKwYBBQUHAQEEeDB2MCQGCCsGAQUFBzABhhhodHRwOi8vb2NzcC5k
# aWdpY2VydC5jb20wTgYIKwYBBQUHMAKGQmh0dHA6Ly9jYWNlcnRzLmRpZ2ljZXJ0
# LmNvbS9ORVRGb3VuZGF0aW9uUHJvamVjdHNDb2RlU2lnbmluZ0NBLmNydDAMBgNV
# HRMBAf8EAjAAMA0GCSqGSIb3DQEBCwUAA4IBAQCgwZvcU0JuY6FZlCQFuHPKr2Ay
# dS1d+SAqK7gT3RCtdzzPJBt8VWhdaEMoHvAbsEdLFPdY9+iIL0LxdiQHUzvE6c37
# gvPpuCsM6ZzM/T4OmUEcKCadYQAssYJ5F2PdvZv+3Rlddp2RtBN3WmNEfbOCRFRB
# jvrR6Oxep8ZJyJAv4fyJrFVSNj/jS7IkyYLQJChC70Rh6UF7JChyEZaG15/NPUUB
# jIq8u5phqDvIGFyem1BV5oeoC6AA2wqcRwobgaq7v8GIaAcVA902B3aLEEKFH1HL
# z1yOs+AEcP+Sf3LXQPM4jITKLXQFhHVGUqSOwIW/RDWMl4gk1eEzLh2Nw4HyMYIR
# HDCCERgCAQEwbjBaMQswCQYDVQQGEwJVUzEYMBYGA1UEChMPLk5FVCBGb3VuZGF0
# aW9uMTEwLwYDVQQDEyguTkVUIEZvdW5kYXRpb24gUHJvamVjdHMgQ29kZSBTaWdu
# aW5nIENBAhAKcaGwwpb1x5BlRwo8IFN+MA0GCWCGSAFlAwQCAQUAoIG0MBkGCSqG
# SIb3DQEJAzEMBgorBgEEAYI3AgEEMBwGCisGAQQBgjcCAQsxDjAMBgorBgEEAYI3
# AgEVMC8GCSqGSIb3DQEJBDEiBCCCOa2QssFupf2WAHdKQdGl9slHZUwdglGXretW
# 6qlzETBIBgorBgEEAYI3AgEMMTowOKASgBAASgBzAG8AbgAuAE4ARQBUoSKAIGh0
# dHBzOi8vd3d3Lm5ld3RvbnNvZnQuY29tL2pzb24gMA0GCSqGSIb3DQEBAQUABIIB
# AAw8fV9xgOhVIOgtThB9pRHFh9inXxG4vziuxcY0KdpfPkQxa5R9qHtXUn96WsQ4
# H9WaX1007fc23QfhIZ1QlpbbtV5L0SZEXRizgRheUC4kpZBARfIt9VVY+GwbJh7X
# PB+fExhhf/9ipahF+nWu02w5bbBdRvP82Dao34cM1OILKJcBwWIvrVbTjMKeRZvh
# G1TtqCDWiIG6JepPmfsQp+09zAQIQzt/sHmLq2wmOaT3dJC7T5+9ez43S+RDjU4I
# YzH1DXMY40ODMnHjp5D0hSL3Mfa1tKJ1UFYJCQ17M8K68lHTy09ux7EqqlQZ500U
# e5we3zFg2cWYc9AXqaxCgJihgg7IMIIOxAYKKwYBBAGCNwMDATGCDrQwgg6wBgkq
# hkiG9w0BBwKggg6hMIIOnQIBAzEPMA0GCWCGSAFlAwQCAQUAMHcGCyqGSIb3DQEJ
# EAEEoGgEZjBkAgEBBglghkgBhv1sBwEwMTANBglghkgBZQMEAgEFAAQgY8HRErjf
# 0Sb+IF62XKn8RwGCKioc5EqsCT70uwKEOB8CEC1tex1y/D4uGo4UM0oZ1WcYDzIw
# MTgxMTI3MTgwNzQzWqCCC7swggaCMIIFaqADAgECAhAJwPxGyARCE7VZi68oT05B
# MA0GCSqGSIb3DQEBCwUAMHIxCzAJBgNVBAYTAlVTMRUwEwYDVQQKEwxEaWdpQ2Vy
# dCBJbmMxGTAXBgNVBAsTEHd3dy5kaWdpY2VydC5jb20xMTAvBgNVBAMTKERpZ2lD
# ZXJ0IFNIQTIgQXNzdXJlZCBJRCBUaW1lc3RhbXBpbmcgQ0EwHhcNMTcwMTA0MDAw
# MDAwWhcNMjgwMTE4MDAwMDAwWjBMMQswCQYDVQQGEwJVUzERMA8GA1UEChMIRGln
# aUNlcnQxKjAoBgNVBAMTIURpZ2lDZXJ0IFNIQTIgVGltZXN0YW1wIFJlc3BvbmRl
# cjCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEBAJ6VmGo0O3MbqH78x74p
# aYnHaCZGXz2NYnOHgaOhnPC3WyQ3WpLU9FnXdonk3NUn8NVmvArutCsxZ6xYxUqR
# WStFHgkB1mSzWe6NZk37I17MEA0LimfvUq6gCJDCUvf1qLVumyx7nee1Pvt4zTJQ
# GL9AtUyMu1f0oE8RRWxCQrnlr9bf9Kd8CmiWD9JfKVfO+x0y//QRoRMi+xLL79dT
# 0uuXy6KsGx2dWCFRgsLC3uorPywihNBD7Ds7P0fE9lbcRTeYtGt0tVmveFdpyA8J
# Anjd2FPBmdtgxJ3qrq/gfoZKXKlYYahedIoBKGhyTqeGnbUCUodwZkjTju+BJMzc
# 2GUCAwEAAaOCAzgwggM0MA4GA1UdDwEB/wQEAwIHgDAMBgNVHRMBAf8EAjAAMBYG
# A1UdJQEB/wQMMAoGCCsGAQUFBwMIMIIBvwYDVR0gBIIBtjCCAbIwggGhBglghkgB
# hv1sBwEwggGSMCgGCCsGAQUFBwIBFhxodHRwczovL3d3dy5kaWdpY2VydC5jb20v
# Q1BTMIIBZAYIKwYBBQUHAgIwggFWHoIBUgBBAG4AeQAgAHUAcwBlACAAbwBmACAA
# dABoAGkAcwAgAEMAZQByAHQAaQBmAGkAYwBhAHQAZQAgAGMAbwBuAHMAdABpAHQA
# dQB0AGUAcwAgAGEAYwBjAGUAcAB0AGEAbgBjAGUAIABvAGYAIAB0AGgAZQAgAEQA
# aQBnAGkAQwBlAHIAdAAgAEMAUAAvAEMAUABTACAAYQBuAGQAIAB0AGgAZQAgAFIA
# ZQBsAHkAaQBuAGcAIABQAGEAcgB0AHkAIABBAGcAcgBlAGUAbQBlAG4AdAAgAHcA
# aABpAGMAaAAgAGwAaQBtAGkAdAAgAGwAaQBhAGIAaQBsAGkAdAB5ACAAYQBuAGQA
# IABhAHIAZQAgAGkAbgBjAG8AcgBwAG8AcgBhAHQAZQBkACAAaABlAHIAZQBpAG4A
# IABiAHkAIAByAGUAZgBlAHIAZQBuAGMAZQAuMAsGCWCGSAGG/WwDFTAfBgNVHSME
# GDAWgBT0tuEgHf4prtLkYaWyoiWyyBc1bjAdBgNVHQ4EFgQU4acySu4BISh9VNXy
# B5JutAcPPYcwcQYDVR0fBGowaDAyoDCgLoYsaHR0cDovL2NybDMuZGlnaWNlcnQu
# Y29tL3NoYTItYXNzdXJlZC10cy5jcmwwMqAwoC6GLGh0dHA6Ly9jcmw0LmRpZ2lj
# ZXJ0LmNvbS9zaGEyLWFzc3VyZWQtdHMuY3JsMIGFBggrBgEFBQcBAQR5MHcwJAYI
# KwYBBQUHMAGGGGh0dHA6Ly9vY3NwLmRpZ2ljZXJ0LmNvbTBPBggrBgEFBQcwAoZD
# aHR0cDovL2NhY2VydHMuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0U0hBMkFzc3VyZWRJ
# RFRpbWVzdGFtcGluZ0NBLmNydDANBgkqhkiG9w0BAQsFAAOCAQEAHvBBgjKu7fG0
# NRPcUMLVl64iIp0ODq8z00z9fL9vARGnlGUiXMYiociJUmuajHNc2V4/Mt4WYEyL
# Nv0xmQq9wYS3jR3viSYTBVbzR81HW62EsjivaiO1ReMeiDJGgNK3ppki/cF4z/WL
# 2AyMBQnuROaA1W1wzJ9THifdKkje2pNlrW5lo5mnwkAOc8xYT49FKOW8nIjmKM5g
# XS0lXYtzLqUNW1Hamk7/UAWJKNryeLvSWHiNRKesOgCReGmJZATTXZbfKr/5pUws
# k//mit2CrPHSs6KGmsFViVZqRz/61jOVQzWJBXhaOmnaIrgEQ9NvaDU2ehQ+RemY
# ZIYPEwwmSjCCBTEwggQZoAMCAQICEAqhJdbWMht+QeQF2jaXwhUwDQYJKoZIhvcN
# AQELBQAwZTELMAkGA1UEBhMCVVMxFTATBgNVBAoTDERpZ2lDZXJ0IEluYzEZMBcG
# A1UECxMQd3d3LmRpZ2ljZXJ0LmNvbTEkMCIGA1UEAxMbRGlnaUNlcnQgQXNzdXJl
# ZCBJRCBSb290IENBMB4XDTE2MDEwNzEyMDAwMFoXDTMxMDEwNzEyMDAwMFowcjEL
# MAkGA1UEBhMCVVMxFTATBgNVBAoTDERpZ2lDZXJ0IEluYzEZMBcGA1UECxMQd3d3
# LmRpZ2ljZXJ0LmNvbTExMC8GA1UEAxMoRGlnaUNlcnQgU0hBMiBBc3N1cmVkIElE
# IFRpbWVzdGFtcGluZyBDQTCCASIwDQYJKoZIhvcNAQEBBQADggEPADCCAQoCggEB
# AL3QMu5LzY9/3am6gpnFOVQoV7YjSsQOB0UzURB90Pl9TWh+57ag9I2ziOSXv2Mh
# kJi/E7xX08PhfgjWahQAOPcuHjvuzKb2Mln+X2U/4Jvr40ZHBhpVfgsnfsCi9aDg
# 3iI/Dv9+lfvzo7oiPhisEeTwmQNtO4V8CdPuXciaC1TjqAlxa+DPIhAPdc9xck4K
# rd9AOly3UeGheRTGTSQjMF287DxgaqwvB8z98OpH2YhQXv1mblZhJymJhFHmgudG
# UP2UKiyn5HU+upgPhH+fMRTWrdXyZMt7HgXQhBlyF/EXBu89zdZN7wZC/aJTKk+F
# HcQdPK/P2qwQ9d2srOlW/5MCAwEAAaOCAc4wggHKMB0GA1UdDgQWBBT0tuEgHf4p
# rtLkYaWyoiWyyBc1bjAfBgNVHSMEGDAWgBRF66Kv9JLLgjEtUYunpyGd823IDzAS
# BgNVHRMBAf8ECDAGAQH/AgEAMA4GA1UdDwEB/wQEAwIBhjATBgNVHSUEDDAKBggr
# BgEFBQcDCDB5BggrBgEFBQcBAQRtMGswJAYIKwYBBQUHMAGGGGh0dHA6Ly9vY3Nw
# LmRpZ2ljZXJ0LmNvbTBDBggrBgEFBQcwAoY3aHR0cDovL2NhY2VydHMuZGlnaWNl
# cnQuY29tL0RpZ2lDZXJ0QXNzdXJlZElEUm9vdENBLmNydDCBgQYDVR0fBHoweDA6
# oDigNoY0aHR0cDovL2NybDQuZGlnaWNlcnQuY29tL0RpZ2lDZXJ0QXNzdXJlZElE
# Um9vdENBLmNybDA6oDigNoY0aHR0cDovL2NybDMuZGlnaWNlcnQuY29tL0RpZ2lD
# ZXJ0QXNzdXJlZElEUm9vdENBLmNybDBQBgNVHSAESTBHMDgGCmCGSAGG/WwAAgQw
# KjAoBggrBgEFBQcCARYcaHR0cHM6Ly93d3cuZGlnaWNlcnQuY29tL0NQUzALBglg
# hkgBhv1sBwEwDQYJKoZIhvcNAQELBQADggEBAHGVEulRh1Zpze/d2nyqY3qzeM8G
# N0CE70uEv8rPAwL9xafDDiBCLK938ysfDCFaKrcFNB1qrpn4J6JmvwmqYN92pDqT
# D/iy0dh8GWLoXoIlHsS6HHssIeLWWywUNUMEaLLbdQLgcseY1jxk5R9IEBhfiThh
# TWJGJIdjjJFSLK8pieV4H9YLFKWA1xJHcLN11ZOFk362kmf7U2GJqPVrlsD0WGkN
# fMgBsbkodbeZY4UijGHKeZR+WfyMD+NvtQEmtmyl7odRIeRYYJu6DC0rbaLEfrvE
# JStHAgh8Sa4TtuF8QkIoxhhWz0E0tmZdtnR79VYzIi8iNrJLokqV2PWmjlIxggJN
# MIICSQIBATCBhjByMQswCQYDVQQGEwJVUzEVMBMGA1UEChMMRGlnaUNlcnQgSW5j
# MRkwFwYDVQQLExB3d3cuZGlnaWNlcnQuY29tMTEwLwYDVQQDEyhEaWdpQ2VydCBT
# SEEyIEFzc3VyZWQgSUQgVGltZXN0YW1waW5nIENBAhAJwPxGyARCE7VZi68oT05B
# MA0GCWCGSAFlAwQCAQUAoIGYMBoGCSqGSIb3DQEJAzENBgsqhkiG9w0BCRABBDAc
# BgkqhkiG9w0BCQUxDxcNMTgxMTI3MTgwNzQzWjAvBgkqhkiG9w0BCQQxIgQgue7/
# KL3RMSPuPb+k95sMry32zF8/UCE4LvX2gcjdpR0wKwYLKoZIhvcNAQkQAgwxHDAa
# MBgwFgQUQAGRR1yYiR3roQSvRwkbXrbUy8swDQYJKoZIhvcNAQEBBQAEggEAUvDa
# mVftYTbTo0bV82fsmj4ZY/X0DcHiUNbYSkdPK3kgpgUn6me05ptlujjXS1DQ9R1X
# qlZFbIawdYnNYe+dMqeX0JJwX0ccLFuz2LGSb8EGbIEDwGy8vDf/lQM17caJX+ZO
# S9+ZR1C82wWg5ZOK1/l8hKBdjNz0Q6PYTkXsZMbaO3o/DC6cmGQ8AfIzWUwAjlJC
# s3P/p8rRjASPlUKD7vfBi6+kiie1VpNcq3QY+I0GJIYLXf+92SeKexvQofSVqvSQ
# Aof6nxugG76L7JNXCpR8Bx9yiDBY041Kxwg4gC4KBMwgUOYNjsMzjPOhhpMrGeyg
# iD3r6OWKJ+B2qHpPEQ==
# SIG # End signature block
