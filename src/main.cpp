#include "gmem/gmem.h"

inline std::uint32_t LOCAL_PLAYER_ADDR = 0x20c1138;
inline std::int32_t HEALTH = 0x43c;

int main()
{
  auto pid = gmem::process::get_pid("R5Apex.exe");
  std::cout << "[gmem-test] - game's pid: " << pid << std::endl;

  auto base_addr = gmem::process::get_base_addr("r5apex.exe", pid);
  std::cout << "[gmem-test] - module base addr: 0x" << std::hex << base_addr << std::endl;

  auto local_player = gmem::process::read_mem<std::uint32_t>(base_addr + LOCAL_PLAYER_ADDR);
  std::cout << "[gmem-test] - local player ptr: 0x" << std::hex << local_player << std::endl;

  auto health = gmem::process::read_mem<std::int32_t>(local_player + HEALTH);
  std::cout << "[gmem-test] - local player health: " << std::dec << health << std::endl;

  return 0;
}