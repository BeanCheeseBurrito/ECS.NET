using System;
using ECS.NET.Collections;
using ECS.NET.Utilities;

namespace ECS.NET.Core
{
    public unsafe struct EntityIndex : IDisposable
    {
        public Vec<ulong> Dense;
        public Vec<Ptr<Record>> Pages;
        public ulong MaxId;
        public ulong AliveCount;

        public const int PageBits = 12;
        public const int PageSize = 1 << PageBits;
        public const int PageMask = PageSize - 1;

        public EntityIndex(Vec<ulong> dense, Vec<Ptr<Record>> pages)
        {
            Dense = dense;
            Pages = pages;
            AliveCount = 1;
            MaxId = 0;
        }

        public static EntityIndex Init()
        {
            Vec<Ptr<Record>> Pages = Vec<Ptr<Record>>.InitZero(1);
            Vec<ulong> Dense = Vec<ulong>.InitZero(1);
            Dense.SetMinCount(1);

            return new EntityIndex(Dense, Pages);
        }

        public void Dispose()
        {
            for (ulong i = 0; i < Pages.Count; i++)
                if (!Pages[i].IsNull)
                    Memory.Free(Pages[i]);

            Pages.Dispose();
            Dense.Dispose();
        }

        private Record* EnsurePage(uint id)
        {
            uint pageIndex = id >> 12;

            if (pageIndex >= Pages.Count)
                Pages.SetMinCount(pageIndex + 1, true);

            Record** pagePtr = (Record**)Pages.PointerAt(pageIndex);
            return *pagePtr != null ? *pagePtr : *pagePtr = Memory.AllocZeroed<Record>(PageSize);
        }

        public Record* GetAny(ulong entity)
        {
            uint id = (uint)entity;
            uint pageIndex = id >> 12;
            Record* page = Pages[pageIndex];
            Record* record = &page[id & PageMask];
            Assert.True(record->Dense != 0);
            return record;
        }

        public Record* Get(ulong entity)
        {
            Record* record = GetAny(entity);
            Assert.True(record->Dense < AliveCount);
            Assert.True(Dense[record->Dense] == entity);
            return record;
        }

        public Record* TryGetAny(ulong entity)
        {
            uint id = (uint)entity;
            uint pageIndex = id >> 12;

            if (pageIndex >= Pages.Count)
                return null;

            Record* page = Pages[pageIndex];

            if (page == null)
                return null;

            Record* record = &page[id & PageMask];

            return record->Dense == 0 ? null : record;
        }

        public Record* TryGet(ulong entity)
        {
            Record* record = TryGetAny(entity);

            if (record == null || record->Dense >= AliveCount || Dense[record->Dense] != entity)
                return null;

            return record;
        }

        public ulong NewId()
        {
            if (AliveCount != Dense.Count)
                return Dense[AliveCount++];

            uint id = (uint)++MaxId;
            Dense.Add(id);

            Record* page = EnsurePage(id);
            Record* record = &page[id & PageMask];
            record->Dense = AliveCount++;

            return id;
        }

        public void Delete(ulong entity)
        {
            Record* record = TryGet(entity);

            if (record == null)
                return;

            ulong deleteIndex = record->Dense;

            ulong lastIndex = --AliveCount;
            ulong lastEntity = Dense[lastIndex];

            Record* lastRecord = GetAny(lastEntity);
            lastRecord->Dense = deleteIndex;

            *record = new Record { Dense = lastIndex };

            Dense[deleteIndex] = lastEntity;
            Dense[lastIndex] = Ecs.IncrementGeneration(entity);
        }

        public bool IsAlive(ulong entity)
        {
            return TryGet(entity) != null;
        }
    }
}