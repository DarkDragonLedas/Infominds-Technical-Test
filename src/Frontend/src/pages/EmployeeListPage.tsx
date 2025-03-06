import {
    Paper,
    Table,
    TableBody,
    TableCell,
    TableContainer,
    TableHead,
    TableRow,
    Typography,
    styled,
    tableCellClasses,
} from "@mui/material";
import { useEffect, useState } from "react";
  
interface EmployeesListQuery {
    id: number;
    code: string;
    firstName: string;
    lastName: string;
    address: string;
    email: string;
    phone: string;
    department?: 
    {
        code: string;
        description: string;
    }
}

export default function EmployeeListPage() {

    const [list, setList] = useState<EmployeesListQuery[]>([]);
    
      useEffect(() => {
        fetch("/api/employees/list")
          .then((response) => {
            return response.json();
          })
          .then((data) => {
            setList(data as EmployeesListQuery[]);
          });
    }, []);

    return (
        <>
          <Typography variant="h4" sx={{ textAlign: "center", mt: 4, mb: 4 }}>
            Employees
          </Typography>
    
          <TableContainer component={Paper}>
            <Table sx={{ minWidth: 2000 }} aria-label="simple table">
              <TableHead>
                <TableRow>
                  <StyledTableHeadCell>Code</StyledTableHeadCell>
                  <StyledTableHeadCell>FirstName</StyledTableHeadCell>
                  <StyledTableHeadCell>LastName</StyledTableHeadCell>
                  <StyledTableHeadCell>Address</StyledTableHeadCell>
                  <StyledTableHeadCell>Email</StyledTableHeadCell>
                  <StyledTableHeadCell>Phone</StyledTableHeadCell>
                  <StyledTableHeadCell>Department-Code</StyledTableHeadCell>
                  <StyledTableHeadCell>Department-Description</StyledTableHeadCell>
                </TableRow>
              </TableHead>
              <TableBody>
                {list.map((row) => (
                  <TableRow
                    key={row.id}
                    sx={{ "&:last-child td, &:last-child th": { border: 0 } }}
                  >
                    <TableCell>{row.code}</TableCell>
                    <TableCell>{row.firstName}</TableCell>
                    <TableCell>{row.lastName}</TableCell>
                    <TableCell>{row.address}</TableCell>
                    <TableCell>{row.email}</TableCell>
                    <TableCell>{row.phone}</TableCell>
                    <TableCell>{row.department?.code}</TableCell>
                    <TableCell>{row.department?.description}</TableCell>
                  </TableRow>
                ))}
              </TableBody>
            </Table>
          </TableContainer>
        </>
    );
}

const StyledTableHeadCell = styled(TableCell)(({ theme }) => ({
    [`&.${tableCellClasses.head}`]: {
      backgroundColor: theme.palette.primary.light,
      color: theme.palette.common.white,
    },
}));