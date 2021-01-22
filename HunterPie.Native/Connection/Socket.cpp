#pragma once
#include "Socket.h"
#include "../libs/MinHook/MinHook.h"
#include <psapi.h>
#include <tchar.h>

using namespace Connection;

Server* Server::_instance;

Server::Server() {}

bool Connection::Server::initialize()
{
    if (isInitialized)
        return true;

    const wchar_t* addr = L"127.0.0.1";
    const unsigned short port = 16969;

    WSADATA wsaData;
    WORD ver = MAKEWORD(2, 2);

    if (WSAStartup(ver, &wsaData) != 0)
    {
        return false;
    }

    SOCKET ListenSocket;
    ListenSocket = socket(AF_INET, SOCK_STREAM, IPPROTO_TCP);
    if (ListenSocket == INVALID_SOCKET)
    {
        WSACleanup();
        return false;
    }

    SOCKADDR_IN addrServer;
    addrServer.sin_family = AF_INET;
    addrServer.sin_port = htons(port);
    InetPton(AF_INET, addr, &addrServer.sin_addr.s_addr);

    ZeroMemory(&addrServer.sin_zero, 8);

    if (bind(ListenSocket, (SOCKADDR*)&addrServer, sizeof(addrServer)) == SOCKET_ERROR)
    {
        closesocket(ListenSocket);
        WSACleanup();
        return false;
    }

    if (listen(ListenSocket, 5) == SOCKET_ERROR)
    {
        closesocket(ListenSocket);
        WSACleanup();
        return false;
    }

    client = accept(ListenSocket, NULL, NULL);

    if (client == INVALID_SOCKET)
    {
        closesocket(ListenSocket);
        WSACleanup();
        return false;
    }

    closesocket(ListenSocket);

    isInitialized = true;

    std::thread([this]()
        {
            using namespace std::chrono;
            char buffer[DEFAULT_BUFFER_SIZE];
            int recvSize;

            while (client != INVALID_SOCKET)
            {
                recvSize = recv(client, buffer, sizeof(buffer), 0);

                if (recvSize > 0)
                {
                    receivePackets(buffer);
                    ZeroMemory(buffer, sizeof(buffer));
                }

                std::this_thread::sleep_for(16ms);
            }
        }).detach();

    return true;
}

void Connection::Server::receivePackets(char buffer[DEFAULT_BUFFER_SIZE])
{
    using namespace Packets;

    I_PACKET packet = *reinterpret_cast<I_PACKET*>(buffer);

    switch (packet.header.opcode)
    {
        case OPCODE::Connect:
        {
            S_CONNECT packet{};
            packet.header.opcode = OPCODE::Connect;
            packet.header.version = 1;
            packet.success = true;

            LOG("-> C_CONNECT\n");
            sendData(&packet, sizeof(packet));
            break;
        }

        case OPCODE::Disconnect:
        {
            LOG("-> C_DISCONNECT\n");

            sendData(new S_DISCONNECT{}, sizeof(packet));

            disableHooks();

            closesocket(client);
            WSACleanup();
            isInitialized = false;

            initialize();
            break;
        }

        case OPCODE::EnableHooks:
        {
            LOG("-> C_ENABLE_HOOKS\n");
            enableHooks();
            break;
        }

        case OPCODE::DisableHooks:
        {
            LOG("-> C_DISABLE_HOOKS\n");
            disableHooks();
            break;
        }

        case OPCODE::QueueInput:
        {
            LOG("-> C_QUEUE_INPUT\n");
            C_QUEUE_INPUT pkt = *reinterpret_cast<C_QUEUE_INPUT*>(buffer);

            Packets::input* toInject = new Packets::input;

            ZeroMemory(toInject, sizeof(Packets::input));

            memcpy(toInject, &pkt.inputs, sizeof(Packets::input));

            inputQueueMutex.lock();

            inputInjectionToQueue.push(toInject);

            inputQueueMutex.unlock();

            break;
        }
    }
}

void Connection::Server::enableHooks()
{
    if (hooksEnabled)
        return;

    MH_Initialize();
    Game::Input::InitializeHooks();

    MH_EnableHook(MH_ALL_HOOKS);

    hooksEnabled = true;
}

void Connection::Server::disableHooks()
{
    if (!hooksEnabled)
        return;

    MH_DisableHook(MH_ALL_HOOKS);
    MH_Uninitialize();

    hooksEnabled = false;
}

Server* Connection::Server::getInstance()
{
    if (!_instance)
        _instance = new Server();

    return _instance;
}

void Connection::Server::sendData(void* data, int size)
{
    send(client, (char*)data, size, 0);
}

Server& Connection::Server::operator=(Server const&)
{
    return *this;
}
