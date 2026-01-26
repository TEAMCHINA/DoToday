import { useEffect, useRef, useState, useSyncExternalStore } from 'react';
import { HubConnection, HubConnectionBuilder, HubConnectionState } from '@microsoft/signalr';
import { useQueryClient } from '@tanstack/react-query';

export type ConnectionStatus = 'connecting' | 'connected' | 'disconnected';

let connectionInstance: HubConnection | null = null;
let connectionPromise: Promise<HubConnection> | null = null;
let currentStatus: ConnectionStatus = 'disconnected';
const statusListeners = new Set<() => void>();

function setStatus(status: ConnectionStatus) {
  currentStatus = status;
  statusListeners.forEach((listener) => listener());
}

function subscribeToStatus(listener: () => void) {
  statusListeners.add(listener);
  return () => statusListeners.delete(listener);
}

function getStatus() {
  return currentStatus;
}

function getConnection(): Promise<HubConnection> {
  if (connectionPromise) {
    return connectionPromise;
  }

  setStatus('connecting');

  connectionPromise = new Promise((resolve) => {
    if (connectionInstance && connectionInstance.state === HubConnectionState.Connected) {
      setStatus('connected');
      resolve(connectionInstance);
      return;
    }

    connectionInstance = new HubConnectionBuilder()
      .withUrl('/hubs/sync')
      .withAutomaticReconnect()
      .build();

    connectionInstance.onreconnecting(() => setStatus('connecting'));
    connectionInstance.onreconnected(() => setStatus('connected'));
    connectionInstance.onclose(() => setStatus('disconnected'));

    connectionInstance
      .start()
      .then(() => {
        setStatus('connected');
        resolve(connectionInstance!);
      })
      .catch((err) => {
        console.error('SignalR connection error:', err);
        setStatus('disconnected');
        connectionPromise = null;
      });
  });

  return connectionPromise;
}

export function useConnectionStatus(): ConnectionStatus {
  return useSyncExternalStore(subscribeToStatus, getStatus);
}

export function useSignalRSync() {
  const queryClient = useQueryClient();

  useEffect(() => {
    let mounted = true;

    getConnection().then((connection) => {
      if (!mounted) return;

      connection.on('ListCreated', () => {
        queryClient.invalidateQueries({ queryKey: ['lists'] });
      });

      connection.on('ListUpdated', (listId: number) => {
        queryClient.invalidateQueries({ queryKey: ['lists'] });
        queryClient.invalidateQueries({ queryKey: ['list', listId] });
      });

      connection.on('ListDeleted', (listId: number) => {
        queryClient.invalidateQueries({ queryKey: ['lists'] });
        queryClient.invalidateQueries({ queryKey: ['list', listId] });
      });
    });

    return () => {
      mounted = false;
    };
  }, [queryClient]);
}

export function useListGroupSync(listId: number) {
  const queryClient = useQueryClient();
  const joinedRef = useRef(false);

  useEffect(() => {
    if (!listId) return;

    let mounted = true;

    getConnection().then((connection) => {
      if (!mounted) return;

      if (!joinedRef.current) {
        connection.invoke('JoinListGroup', listId);
        joinedRef.current = true;
      }

      const handleTaskCreated = (eventListId: number) => {
        if (eventListId === listId) {
          queryClient.invalidateQueries({ queryKey: ['list', listId] });
        }
      };

      const handleTaskUpdated = (eventListId: number) => {
        if (eventListId === listId) {
          queryClient.invalidateQueries({ queryKey: ['list', listId] });
        }
      };

      connection.on('TaskCreated', handleTaskCreated);
      connection.on('TaskUpdated', handleTaskUpdated);

      return () => {
        connection.off('TaskCreated', handleTaskCreated);
        connection.off('TaskUpdated', handleTaskUpdated);
      };
    });

    return () => {
      mounted = false;
      if (joinedRef.current && connectionInstance?.state === HubConnectionState.Connected) {
        connectionInstance.invoke('LeaveListGroup', listId);
        joinedRef.current = false;
      }
    };
  }, [listId, queryClient]);
}
