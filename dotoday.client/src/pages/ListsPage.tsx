import { useEffect, useState } from 'react';
import { Link } from 'react-router-dom';
import {
  Container,
  Typography,
  Table,
  TableBody,
  TableCell,
  TableContainer,
  TableHead,
  TableRow,
  Paper,
  IconButton,
  Box,
} from '@mui/material';
import DeleteIcon from '@mui/icons-material/Delete';
import { AddItemInput } from '@/components/AddItemInput';
import { DoTodayApiClient, TaskListSummaryDto, CreateTaskListRequest } from '@/api/DoTodayApiClient';

const apiClient = new DoTodayApiClient();

export function ListsPage() {
  const [lists, setLists] = useState<TaskListSummaryDto[]>([]);
  const [loading, setLoading] = useState(true);

  const fetchLists = async () => {
    try {
      const response = await apiClient.getLists();
      setLists(response.lists ?? []);
    } catch (error) {
      console.error('Failed to fetch lists:', error);
    } finally {
      setLoading(false);
    }
  };

  useEffect(() => {
    fetchLists();
  }, []);

  const handleAddList = async (name: string) => {
    try {
      const request = new CreateTaskListRequest({ name });
      await apiClient.createList(request);
      await fetchLists();
    } catch (error) {
      console.error('Failed to create list:', error);
    }
  };

  const handleDeleteList = async (id: number) => {
    try {
      await apiClient.deleteList(id);
      await fetchLists();
    } catch (error) {
      console.error('Failed to delete list:', error);
    }
  };

  if (loading) {
    return (
      <Container maxWidth="md" sx={{ mt: 4 }}>
        <Typography>Loading...</Typography>
      </Container>
    );
  }

  return (
    <Container maxWidth="md" sx={{ mt: 4 }}>
      <Typography variant="h4" component="h1" gutterBottom>
        My Lists
      </Typography>

      <Box sx={{ mb: 3 }}>
        <AddItemInput placeholder="New list name" onAdd={handleAddList} />
      </Box>

      <TableContainer component={Paper}>
        <Table>
          <TableHead>
            <TableRow>
              <TableCell>Name</TableCell>
              <TableCell align="right">Actions</TableCell>
            </TableRow>
          </TableHead>
          <TableBody>
            {lists.length === 0 ? (
              <TableRow>
                <TableCell colSpan={2} align="center">
                  No lists yet. Create one above!
                </TableCell>
              </TableRow>
            ) : (
              lists.map((list) => (
                <TableRow key={list.id}>
                  <TableCell>
                    <Link to={`/lists/${list.id}`} style={{ textDecoration: 'none', color: 'inherit' }}>
                      {list.name}
                    </Link>
                  </TableCell>
                  <TableCell align="right">
                    <IconButton
                      onClick={() => handleDeleteList(list.id!)}
                      color="error"
                      size="small"
                    >
                      <DeleteIcon />
                    </IconButton>
                  </TableCell>
                </TableRow>
              ))
            )}
          </TableBody>
        </Table>
      </TableContainer>
    </Container>
  );
}
