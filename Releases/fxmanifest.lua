fx_version 'adamant'
game 'gta5'

author 'mmleczek (mbinary.pl)'
description 'mmNetworking resource for simple spawning vehicles by serverside'
version '0.0.3'

client_script 'mmNetworkingC.net.dll'
server_script 'mmNetworkingS.net.dll'

exports {
    'CreateVehicle',
}