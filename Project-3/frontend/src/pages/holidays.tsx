import { useQuery } from "@tanstack/react-query";
import { holidaysApi } from "@/lib/api";
import { Card, CardContent } from "@/components/ui/card";
import { Table, TableBody, TableCell, TableHead, TableHeader, TableRow } from "@/components/ui/table";
import { formatDate } from "@/lib/utils";
import { useState } from "react";
import { SelectNative } from "@/components/ui/select-native";

export default function HolidaysPage() {
  const currentYear = new Date().getFullYear();
  const [year, setYear] = useState(currentYear);

  const { data: holidays, isLoading } = useQuery({
    queryKey: ['holidays', year],
    queryFn: () => holidaysApi.getByYear(year),
  });

  return (
    <div className="space-y-6">
      <div className="flex flex-col sm:flex-row justify-between items-start sm:items-center gap-4">
        <div>
          <h1 className="text-3xl font-bold tracking-tight">Public Holidays</h1>
          <p className="text-muted-foreground mt-1">Official non-working days.</p>
        </div>
        <div className="w-32">
          <SelectNative value={year} onChange={(e) => setYear(Number(e.target.value))}>
            {[currentYear - 1, currentYear, currentYear + 1].map(y => (
              <option key={y} value={y}>{y}</option>
            ))}
          </SelectNative>
        </div>
      </div>

      <Card>
        <CardContent className="p-0">
          <Table>
            <TableHeader>
              <TableRow>
                <TableHead className="w-[200px]">Date</TableHead>
                <TableHead>Holiday Name</TableHead>
                <TableHead>Country Code</TableHead>
              </TableRow>
            </TableHeader>
            <TableBody>
              {isLoading ? (
                <TableRow>
                  <TableCell colSpan={3} className="text-center py-8">Loading...</TableCell>
                </TableRow>
              ) : holidays && holidays.length > 0 ? (
                holidays.map((h, i) => (
                  <TableRow key={i}>
                    <TableCell className="font-medium">{formatDate(h.date)}</TableCell>
                    <TableCell>{h.name}</TableCell>
                    <TableCell>{h.countryCode}</TableCell>
                  </TableRow>
                ))
              ) : (
                <TableRow>
                  <TableCell colSpan={3} className="text-center py-8 text-muted-foreground">
                    No holidays found for {year}.
                  </TableCell>
                </TableRow>
              )}
            </TableBody>
          </Table>
        </CardContent>
      </Card>
    </div>
  );
}
