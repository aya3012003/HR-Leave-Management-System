import { useState } from "react";
import { useQuery } from "@tanstack/react-query";
import { employeesApi, departmentsApi } from "@/lib/api";
import { useAuth } from "@/contexts/useAuth";
import { Card, CardContent } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { Button } from "@/components/ui/button";
import { Input } from "@/components/ui/input";
import { SelectNative } from "@/components/ui/select-native";
import { Link } from "wouter";
import { Search, Plus } from "lucide-react";
import { formatDate } from "@/lib/utils";

export default function EmployeesPage() {
  const { isAdmin } = useAuth();
  const [page, setPage] = useState(1);
  const [search, setSearch] = useState("");
  const [deptId, setDeptId] = useState("");

  const { data: pageData, isLoading } = useQuery({
    queryKey: ['employees', page, search, deptId],
    queryFn: () => employeesApi.getAll({ 
      page, 
      pageSize: 10, 
      search: search || undefined, 
      deptId: deptId ? Number(deptId) : undefined 
    }),
  });

  const { data: depts } = useQuery({
    queryKey: ['departments'],
    queryFn: departmentsApi.getAll,
  });

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Employees</h1>
          <p className="text-muted-foreground mt-1">Manage staff records and information.</p>
        </div>
        {isAdmin && (
          <Link href="/employees/new" className="inline-flex">
            <Button className="gap-2"><Plus size={16} /> Add Employee</Button>
          </Link>
        )}
      </div>

      <div className="flex flex-col sm:flex-row gap-4 mb-2">
        <div className="relative w-full sm:max-w-sm">
          <Search className="absolute left-2.5 top-2.5 h-4 w-4 text-muted-foreground" />
          <Input 
            placeholder="Search name or email..." 
            className="pl-9"
            value={search}
            onChange={(e) => { setSearch(e.target.value); setPage(1); }}
          />
        </div>
        <div className="w-full sm:w-48">
          <SelectNative 
            value={deptId} 
            onChange={(e) => { setDeptId(e.target.value); setPage(1); }}
          >
            <option value="">All Departments</option>
            {depts?.map(d => (
              <option key={d.id} value={d.id}>{d.name}</option>
            ))}
          </SelectNative>
        </div>
      </div>

      <Card>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead>Name</TableHead>
                <TableHead>Email</TableHead>
                <TableHead>Department</TableHead>
                <TableHead>Role</TableHead>
                <TableHead>Hire Date</TableHead>
                <TableHead className="text-right">Action</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {isLoading ? (
                <TableRow><TableCell colSpan={6} className="text-center py-8">Loading...</TableCell></TableRow>
              ) : pageData?.items && pageData.items.length > 0 ? (
                pageData.items.map(emp => (
                  <TableRow key={emp.id}>
                    <TableCell className="font-medium">{emp.fullName}</TableCell>
                    <TableCell>{emp.email}</TableCell>
                    <TableCell>{emp.departmentName || "-"}</TableCell>
                    <TableCell>
                      <div className="flex gap-1 flex-wrap">
                        {emp.roles.map(r => (
                          <span key={r} className="px-2 py-0.5 bg-secondary text-secondary-foreground text-xs rounded-full">{r}</span>
                        ))}
                      </div>
                    </TableCell>
                    <TableCell>{formatDate(emp.hireDate)}</TableCell>
                    <TableCell className="text-right">
                      <Link href={`/employees/${emp.id}`}>
                        <Button variant="ghost" size="sm">View</Button>
                      </Link>
                    </TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow><TableCell colSpan={6} className="text-center py-8 text-muted-foreground">No employees found.</TableCell></TableRow>
              )}
            </TableBody>
          </Table>
          
          {pageData && pageData.totalPages > 1 && (
            <div className="flex items-center justify-between px-6 py-3 border-t">
              <span className="text-sm text-muted-foreground">
                Page {pageData.pageNumber} of {pageData.totalPages}
              </span>
              <div className="space-x-2">
                <Button variant="outline" size="sm" disabled={page === 1} onClick={() => setPage(p => p - 1)}>Previous</Button>
                <Button variant="outline" size="sm" disabled={page === pageData.totalPages} onClick={() => setPage(p => p + 1)}>Next</Button>
              </div>
            </div>
          )}
        </CardContent>
      </Card>
    </div>
  );
}
