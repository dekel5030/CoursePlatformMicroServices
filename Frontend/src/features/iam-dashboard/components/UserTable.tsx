import { useState, useMemo } from "react";
import {
  useReactTable,
  getCoreRowModel,
  getFilteredRowModel,
  getSortedRowModel,
  flexRender,
  type ColumnDef,
  type SortingState,
} from "@tanstack/react-table";
import { Search, ChevronUp, ChevronDown, Edit2 } from "lucide-react";
import {
  Table,
  TableHeader,
  TableBody,
  TableRow,
  TableHead,
  TableCell,
} from "@/components";
import { Badge, Input, Button } from "@/components";
import type { UserDto } from "../types/UserDto";
import { useTranslation } from "react-i18next";
import { motion, AnimatePresence } from "framer-motion";

interface UserTableProps {
  users: UserDto[];
  onUserSelect: (user: UserDto) => void;
}

export default function UserTable({ users, onUserSelect }: UserTableProps) {
  const { t } = useTranslation();
  const [globalFilter, setGlobalFilter] = useState("");
  const [sorting, setSorting] = useState<SortingState>([]);

  const columns = useMemo<ColumnDef<UserDto>[]>(
    () => [
      {
        accessorKey: "firstName",
        header: t("authManagement.users.table.name"),
        cell: ({ row }) => (
          <div className="font-medium">
            {row.original.firstName} {row.original.lastName}
          </div>
        ),
      },
      {
        accessorKey: "email",
        header: t("authManagement.users.table.email"),
        cell: ({ row }) => (
          <span className="text-muted-foreground">{row.original.email}</span>
        ),
      },
      {
        id: "roles",
        header: t("authManagement.users.table.assignedRoles"),
        accessorFn: (row) => row.roles.map((r) => r.name).join(", "),
        cell: ({ row }) => (
          <div className="flex flex-wrap gap-1">
            {row.original.roles.length === 0 ? (
              <span className="text-sm text-muted-foreground">
                {t("authManagement.users.table.noRoles")}
              </span>
            ) : (
              row.original.roles.map((role) => (
                <Badge
                  key={role.id}
                  variant="secondary"
                  className="bg-secondary/50 hover:bg-secondary"
                >
                  {role.name}
                </Badge>
              ))
            )}
          </div>
        ),
      },
      {
        id: "actions",
        header: t("authManagement.users.table.actions"),
        cell: ({ row }) => (
          <Button
            variant="ghost"
            size="sm"
            onClick={() => onUserSelect(row.original)}
            className="gap-2 hover:bg-primary/10 hover:text-primary"
          >
            <Edit2 className="h-4 w-4" />
            {t("authManagement.users.table.edit")}
          </Button>
        ),
      },
    ],
    [onUserSelect, t]
  );

  const table = useReactTable({
    data: users,
    columns,
    state: {
      globalFilter,
      sorting,
    },
    onGlobalFilterChange: setGlobalFilter,
    onSortingChange: setSorting,
    getCoreRowModel: getCoreRowModel(),
    getFilteredRowModel: getFilteredRowModel(),
    getSortedRowModel: getSortedRowModel(),
  });

  return (
    <div className="space-y-4">
      <div className="relative">
        <Search className="absolute left-3 top-1/2 -translate-y-1/2 h-4 w-4 text-muted-foreground" />
        <Input
          type="text"
          placeholder={t("authManagement.users.searchPlaceholder")}
          value={globalFilter}
          onChange={(e) => setGlobalFilter(e.target.value)}
          className="pl-10"
        />
      </div>

      <div className="rounded-lg border border-border overflow-hidden bg-card shadow-sm">
        <Table>
          <TableHeader>
            {table.getHeaderGroups().map((headerGroup) => (
              <TableRow key={headerGroup.id} className="hover:bg-transparent">
                {headerGroup.headers.map((header) => (
                  <TableHead
                    key={header.id}
                    onClick={header.column.getToggleSortingHandler()}
                    className={
                      header.column.getCanSort()
                        ? "cursor-pointer select-none"
                        : ""
                    }
                  >
                    <div className="flex items-center gap-2">
                      {flexRender(
                        header.column.columnDef.header,
                        header.getContext()
                      )}
                      {header.column.getCanSort() && (
                        <span className="text-muted-foreground">
                          {header.column.getIsSorted() === "asc" ? (
                            <ChevronUp className="h-4 w-4" />
                          ) : header.column.getIsSorted() === "desc" ? (
                            <ChevronDown className="h-4 w-4" />
                          ) : (
                            <span className="text-xs opacity-50">⇅</span>
                          )}
                        </span>
                      )}
                    </div>
                  </TableHead>
                ))}
              </TableRow>
            ))}
          </TableHeader>
          <TableBody>
            <AnimatePresence>
              {table.getRowModel().rows.length === 0 ? (
                <TableRow>
                  <TableCell
                    colSpan={columns.length}
                    className="text-center py-8"
                  >
                    <motion.div
                      initial={{ opacity: 0 }}
                      animate={{ opacity: 1 }}
                      exit={{ opacity: 0 }}
                      className="space-y-2"
                    >
                      <p className="text-muted-foreground">
                        {t("authManagement.users.noUsers")}
                      </p>
                      {globalFilter && (
                        <Button
                          variant="link"
                          onClick={() => setGlobalFilter("")}
                        >
                          {t("authManagement.users.clearSearch")}
                        </Button>
                      )}
                    </motion.div>
                  </TableCell>
                </TableRow>
              ) : (
                table.getRowModel().rows.map((row, index) => (
                  <motion.tr
                    key={row.id}
                    initial={{ opacity: 0, y: 10 }}
                    animate={{ opacity: 1, y: 0 }}
                    transition={{ delay: index * 0.05 }}
                    className="border-b transition-colors hover:bg-muted/50 data-[state=selected]:bg-muted"
                  >
                    {row.getVisibleCells().map((cell) => (
                      <TableCell key={cell.id}>
                        {flexRender(
                          cell.column.columnDef.cell,
                          cell.getContext()
                        )}
                      </TableCell>
                    ))}
                  </motion.tr>
                ))
              )}
            </AnimatePresence>
          </TableBody>
        </Table>
      </div>

      {table.getRowModel().rows.length > 0 && (
        <div className="text-sm text-muted-foreground">
          {t("authManagement.users.showingUsers", {
            count: table.getRowModel().rows.length,
            total: users.length,
          })}
        </div>
      )}
    </div>
  );
}
